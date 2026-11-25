using System;
using R3;

namespace DIVFactor.Lifecycle
{
    public sealed class ViewContext
    {
        readonly CompletionPhase phase;
        internal ViewContext(CompletionPhase phase) => this.phase = phase;
        public Observable<Unit> UpdatedView => phase.Signal;
        public void Fail(Exception exception) => phase.Fail(exception);
    }

    public sealed class SubscriptionContext
    {
        readonly CompletionPhase phase;
        public UsageLifetime Lifetime { get; }
        internal SubscriptionContext(UsageLifetime lifetime, CompletionPhase phase)
        {
            Lifetime = lifetime;
            this.phase = phase;
        }
        public Observable<Unit> Subscribed => phase.Signal;
        public void Fail(Exception exception) => phase.Fail(exception);
    }

    public static class LifecycleSubscriptionExtensions
    {
        // R3は購読側の例外をOnErrorResumeへ流す。失敗を同じ開始処理へ戻して表示を止める。
        public static IDisposable SubscribeLifecycle(this Observable<Unit> signal, Action onReady, Action<Exception> fail)
        {
            if (onReady == null) throw new ArgumentNullException(nameof(onReady));
            if (fail == null) throw new ArgumentNullException(nameof(fail));
            if (ReferenceEquals(signal, Observable.Empty<Unit>())) return Disposable.Empty;
            return signal.SubscribeLifecycle(_ => onReady(), fail);
        }

        public static IDisposable SubscribeLifecycle(this Observable<Unit> signal, Action<Unit> onReady, Action<Exception> fail)
        {
            if (onReady == null) throw new ArgumentNullException(nameof(onReady));
            if (fail == null) throw new ArgumentNullException(nameof(fail));
            if (ReferenceEquals(signal, Observable.Empty<Unit>())) return Disposable.Empty;
            return signal.Subscribe(onReady, fail, result => { if (result.IsFailure) fail(result.Exception); });
        }
    }

    // 使用ごと・段階ごとに独立させる。購読する対象がなければSubjectは不要。
    internal sealed class CompletionPhase : IDisposable
    {
        Func<bool> canContinue;
        Action<Exception> reportFailure;
        Subject<Unit> source;
        Observable<Unit> signal;
        bool closed;

        internal CompletionPhase(Func<bool> canContinue, Action<Exception> reportFailure)
        {
            this.canContinue = canContinue;
            this.reportFailure = reportFailure;
        }

        internal Observable<Unit> Signal
        {
            get
            {
                if (closed) return Observable.Empty<Unit>();
                if (signal == null)
                {
                    source = new Subject<Unit>();
                    // 先の購読がReleaseまたは失敗を通知したら、後続へOnNextを送らない。
                    signal = source.Where(_ => !closed && canContinue()).Take(1);
                }
                return signal;
            }
        }

        internal void Fail(Exception exception)
        {
            if (!closed) reportFailure(exception ?? throw new ArgumentNullException(nameof(exception)));
        }

        internal void Publish()
        {
            if (closed) return;
            try { if (source != null && canContinue()) source.OnNext(Unit.Default); }
            finally { Dispose(); }
        }

        public void Dispose()
        {
            if (closed) return;
            closed = true;
            // 中止されてTake(1)に届かなかった購読も、値を通知せずに閉じる。
            var pending = source;
            signal = null;
            source = null;
            canContinue = null;
            reportFailure = null;
            pending?.OnCompleted();
        }
    }
}
