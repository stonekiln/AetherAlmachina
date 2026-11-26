using DIVFactor.Injectable;
using R3;
using System;
using System.Collections.Generic;

namespace DIVFactor.Lifecycle
{
    public enum LifecycleState { Created, Starting, Active, Releasing, Released, Destroying, Disposed }

    /// <summary>対象一覧と固定コンテナを維持し、使用期間ごとに開始・終了する。</summary>
    public sealed class LifecycleRunner : IDisposable
    {
        readonly LifecycleParticipants participants;
        readonly Action show;
        readonly Action hide;
        readonly Action destroy;
        readonly Action beforeRelease;
        readonly Action returnToPool;
        readonly Action clearArguments;
        readonly UsageLifetime management = new();
        UsageLifetime usage;
        LifecycleEventHub usageEvents;
        InjectableResolver activeResolver;
        bool running;
        bool subscriptionsReady;
        Action<Exception> failStartingSpawn;
        bool endRequested;
        bool destroyRequested;
        bool disposeRequested;
        bool began;
        bool injected;

        public LifecycleState State { get; private set; }

        public LifecycleRunner(IEnumerable<object> participants, LifecycleEventHub events,
            Action show, Action hide, Action destroy, Action beforeRelease = null,
            Action returnToPool = null, Action clearArguments = null)
            : this(LifecycleParticipants.From(participants), events, show, hide, destroy, beforeRelease, returnToPool, clearArguments) { }

        public LifecycleRunner(LifecycleParticipants participants, LifecycleEventHub events,
            Action show, Action hide, Action destroy, Action beforeRelease = null,
            Action returnToPool = null, Action clearArguments = null)
        {
            this.participants = participants ?? throw new ArgumentNullException(nameof(participants));
            this.show = show;
            this.hide = hide;
            this.destroy = destroy ?? throw new ArgumentNullException(nameof(destroy));
            this.beforeRelease = beforeRelease;
            this.returnToPool = returnToPool;
            this.clearArguments = clearArguments;
            if (events == null) throw new ArgumentNullException(nameof(events));
            // Containerから直接取得するHubは物理個体の管理用。通常のInjectionには期間専用Hubを渡す。
            events.Release.Subscribe(_ => RequestEnd(false)).AddTo(management);
            events.Destroy.Subscribe(_ => RequestEnd(true)).AddTo(management);
            events.Spawn.Subscribe(Spawn).AddTo(management);
        }

        public void Run(InjectableResolver resolver)
        {
            if (State != LifecycleState.Created && !(State == LifecycleState.Released && returnToPool != null))
                throw new InvalidOperationException("この個体は開始できる状態ではありません。");
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            usage = new UsageLifetime();
            usageEvents = CreateUsageEvents(usage);
            endRequested = destroyRequested = disposeRequested = injected = subscriptionsReady = false;
            began = running = true;
            State = LifecycleState.Starting;
            Exception failure = null;
            bool CanContinue() => !endRequested;
            void FailStart(Exception exception)
            {
                failure = Combine(failure, exception);
                endRequested = destroyRequested = true;
            }
            try
            {
                failStartingSpawn = FailStart;
                using var injection = new CompletionPhase(CanContinue, FailStart);
                activeResolver = resolver.WithLifecycleEvents(usageEvents, injection);
                RunPhase(participants.Injectables, target => target.Injection(activeResolver));
                injected = !endRequested;
                injection.Publish();

                if (!endRequested && participants.Views.Length != 0)
                {
                    using var view = new CompletionPhase(CanContinue, FailStart);
                    var context = new ViewContext(view);
                    RunPhase(participants.Views, target => target.UpdateView(context));
                    view.Publish();
                }
                if (!endRequested && participants.Subscribers.Length != 0)
                {
                    using var subscribed = new CompletionPhase(CanContinue, FailStart);
                    var context = new SubscriptionContext(usage, subscribed);
                    RunPhase(participants.Subscribers, target => target.Subscribe(context));
                    subscriptionsReady = !endRequested;
                    subscribed.Publish();
                }
                if (!endRequested)
                {
                    subscriptionsReady = true;
                    show?.Invoke();
                    if (!endRequested) State = LifecycleState.Active;
                }
            }
            catch (Exception exception) { FailStart(exception); }
            finally { failStartingSpawn = null; running = false; }
            if (endRequested)
            {
                try { FinishEnd(); }
                catch (Exception exception) { failure = Combine(failure, exception); }
            }
            if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        }

        LifecycleEventHub CreateUsageEvents(UsageLifetime current)
        {
            LifecycleEventHub events = new(new(), new(), new());
            events.Release.Subscribe(_ => { if (ReferenceEquals(usage, current) && !current.IsDisposed) RequestEnd(false); }).AddTo(current);
            events.Destroy.Subscribe(_ => { if (ReferenceEquals(usage, current) && !current.IsDisposed) RequestEnd(true); }).AddTo(current);
            events.Spawn.Subscribe(request => { if (ReferenceEquals(usage, current) && !current.IsDisposed) Spawn(request); }).AddTo(current);
            return events;
        }

        void Spawn(SpawnRequest request)
        {
            if (endRequested || !(State == LifecycleState.Active || (State == LifecycleState.Starting && subscriptionsReady))) return;
            try
            {
                if (request.Factory == null) throw new ArgumentNullException(nameof(request.Factory));
                request.Factory();
            }
            catch (Exception exception) when (State == LifecycleState.Starting && failStartingSpawn != null)
            {
                // EventPortを経由したFactoryの例外はR3の内側にあるため、開始処理へ明示的に戻す。
                failStartingSpawn(exception);
            }
        }

        void RunPhase<T>(IEnumerable<T> targets, Action<T> action)
        {
            foreach (T target in targets)
            {
                if (endRequested) break;
                action(target);
            }
        }

        internal void EndForOwner() => RequestEnd(false);

        void RequestEnd(bool permanently)
        {
            if (State == LifecycleState.Destroying || State == LifecycleState.Disposed) return;
            if (State == LifecycleState.Released && !permanently) return;
            endRequested = true;
            destroyRequested |= permanently;
            if (!running && State != LifecycleState.Releasing) FinishEnd();
        }

        void FinishEnd()
        {
            if (State == LifecycleState.Destroying || State == LifecycleState.Disposed) return;
            if (began && !injected) destroyRequested = true;
            Exception failure = null;
            try { EndUse(); }
            catch (Exception exception) { failure = exception; destroyRequested = true; }
            try
            {
                if (destroyRequested || returnToPool == null)
                {
                    State = LifecycleState.Destroying;
                    destroy();
                }
                else returnToPool();
            }
            catch (Exception exception)
            {
                failure = Combine(failure, exception);
                if (State != LifecycleState.Destroying && State != LifecycleState.Disposed)
                {
                    State = LifecycleState.Destroying;
                    try { destroy(); }
                    catch (Exception cleanup) { failure = Combine(failure, cleanup); }
                }
            }
            if (disposeRequested)
            {
                try { CompleteDispose(); }
                catch (Exception exception) { failure = Combine(failure, exception); }
            }
            if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        }

        void EndUse()
        {
            if (!began) return;
            State = LifecycleState.Releasing;
            List<Exception> failures = new();
            Attempt(beforeRelease, failures);
            Attempt(() => usage.Dispose(), failures);
            for (int index = participants.Releasables.Length - 1; index >= 0; index--)
                Attempt(participants.Releasables[index].Release, failures);
            Attempt(hide, failures);
            // 完了したHubはDisposeせず完了状態に保つ。遅延した古いOnNextを安全に無視する。
            Attempt(() => usageEvents.Release.OnCompleted(), failures);
            Attempt(() => usageEvents.Destroy.OnCompleted(), failures);
            Attempt(() => usageEvents.Spawn.OnCompleted(), failures);
            Attempt(clearArguments, failures);
            if (returnToPool != null && injected)
                foreach (IInjectable target in participants.Injectables)
                    Attempt(() => target.Injection(activeResolver), failures);
            began = injected = subscriptionsReady = false;
            activeResolver = null;
            usageEvents = null;
            usage = null;
            State = LifecycleState.Released;
            if (failures.Count != 0) throw new AggregateException(failures);
        }

        public void Dispose()
        {
            if (State == LifecycleState.Disposed) return;
            endRequested = destroyRequested = disposeRequested = true;
            if (running) return;
            try { EndUse(); }
            finally { CompleteDispose(); }
        }

        void CompleteDispose()
        {
            try { management.Dispose(); }
            finally { State = LifecycleState.Disposed; }
        }

        static void Attempt(Action action, List<Exception> failures)
        {
            try { action?.Invoke(); }
            catch (Exception exception) { failures.Add(exception); }
        }
        static Exception Combine(Exception first, Exception second)
            => first == null ? second : new AggregateException(first, second);
    }
}
