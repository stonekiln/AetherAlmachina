using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using NUnit.Framework;
using R3;
using System;
using System.Collections.Generic;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    public class LifecycleTests
    {
        static IObjectResolver Build()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<LifecycleEventHub>();
            return builder.Build();
        }

        [Test]
        public void AllTargetsFinishEachPhaseBeforeTheNextPhase()
        {
            using IObjectResolver container = Build();
            List<string> trace = new();
            Probe first = new("A", trace), second = new("B", trace);
            using LifecycleRunner runner = new(new[] { first, second }, container.Resolve<LifecycleEventHub>(),
                () => trace.Add("show"), null, () => trace.Add("destroy"));
            runner.Run(new(container));
            Assert.That(trace, Is.EqualTo(new[] { "A.inject", "B.inject", "A.initialize", "B.initialize",
                "A.view", "B.view", "A.updated", "B.updated", "A.subscribe", "B.subscribe", "A.activate", "B.activate", "show" }));
            Assert.That(runner.State, Is.EqualTo(LifecycleState.Active));
        }

        [Test]
        public void ReleaseDisposesSubscriptionsBeforeHooksAndDestroysOnlyOnce()
        {
            using IObjectResolver container = Build();
            LifecycleEventHub events = container.Resolve<LifecycleEventHub>();
            List<string> trace = new();
            Probe probe = new("A", trace);
            int destroyed = 0;
            using LifecycleRunner runner = new(new[] { probe }, events, null, () => trace.Add("hide"), () => destroyed++);
            runner.Run(new(container));
            probe.Signal.OnNext(1);
            Assert.That(probe.Received, Is.EqualTo(1));
            events.Release.OnNext(new());
            probe.Signal.OnNext(1);
            events.Release.OnNext(new());
            events.Destroy.OnNext(new());
            Assert.That(probe.Received, Is.EqualTo(1));
            Assert.That(probe.ReleasedWithClosedUsage, Is.True);
            Assert.That(trace.FindIndex(value => value == "A.release"), Is.LessThan(trace.IndexOf("hide")));
            Assert.That(destroyed, Is.EqualTo(1));
            runner.Dispose();
            Assert.That(trace.FindAll(value => value == "A.release").Count, Is.EqualTo(1));
        }

        [Test]
        public void ScopedControlEventsDoNotReleaseParentOrSibling()
        {
            using IObjectResolver parent = Build();
            using IScopedObjectResolver child = parent.CreateScope(builder => builder.RegisterEventHub<LifecycleEventHub>());
            using IScopedObjectResolver sibling = parent.CreateScope(builder => builder.RegisterEventHub<LifecycleEventHub>());
            int parentDestroyed = 0, childDestroyed = 0, siblingDestroyed = 0;
            using LifecycleRunner parentRunner = new(Array.Empty<object>(), parent.Resolve<LifecycleEventHub>(), null, null, () => parentDestroyed++);
            using LifecycleRunner childRunner = new(Array.Empty<object>(), child.Resolve<LifecycleEventHub>(), null, null, () => childDestroyed++);
            using LifecycleRunner siblingRunner = new(Array.Empty<object>(), sibling.Resolve<LifecycleEventHub>(), null, null, () => siblingDestroyed++);
            parentRunner.Run(new(parent)); childRunner.Run(new(child)); siblingRunner.Run(new(sibling));
            child.Resolve<LifecycleEventHub>().Release.OnNext(new());
            Assert.That((parentDestroyed, childDestroyed, siblingDestroyed), Is.EqualTo((0, 1, 0)));
        }

        [TestCase("initialize")]
        [TestCase("updated")]
        [TestCase("activate")]
        public void SynchronousEndRequestStopsRemainingWorkWithoutReentry(string phase)
        {
            using IObjectResolver container = Build();
            LifecycleEventHub events = container.Resolve<LifecycleEventHub>();
            List<string> trace = new();
            Probe first = new("A", trace), second = new("B", trace);
            first.Action = current => { if (current == phase) events.Release.OnNext(new()); };
            using LifecycleRunner runner = new(new[] { first, second }, events, null, null, () => trace.Add("destroy"));
            runner.Run(new(container));
            Assert.That(trace, Does.Not.Contain("B." + phase));
            Assert.That(trace.FindAll(value => value == "destroy").Count, Is.EqualTo(1));
            Assert.That(trace.IndexOf("A.release"), Is.GreaterThan(trace.IndexOf("A." + phase)));
        }

        [TestCase("initialize")]
        [TestCase("updated")]
        [TestCase("activate")]
        public void FailedCallbackStopsOtherCallbacksAndDiscardsTheInstance(string failingPhase)
        {
            using IObjectResolver container = Build();
            List<string> trace = new();
            Probe probe = new("A", trace) { Action = phase => { if (phase == failingPhase) throw new InvalidOperationException("failed"); } };
            using LifecycleRunner runner = new(new[] { probe, new Probe("B", trace) }, container.Resolve<LifecycleEventHub>(),
                () => trace.Add("show"), null, () => trace.Add("destroy"), returnToPool: () => trace.Add("pool"));
            Assert.Throws<InvalidOperationException>(() => runner.Run(new(container)));
            Assert.That(trace, Does.Not.Contain("B." + failingPhase));
            Assert.That(trace, Does.Not.Contain("show"));
            Assert.That(trace, Does.Not.Contain("pool"));
            Assert.That(trace, Does.Contain("A.release").And.Contain("destroy"));
        }

        [Test]
        public void ReleaseFailureDoesNotSkipOtherTargetsOrHide()
        {
            using IObjectResolver container = Build();
            List<string> trace = new();
            Probe first = new("A", trace), second = new("B", trace) { Action = phase => { if (phase == "release") throw new InvalidOperationException(); } };
            LifecycleRunner runner = new(new[] { first, second }, container.Resolve<LifecycleEventHub>(), null, () => trace.Add("hide"), () => { });
            runner.Run(new(container));
            Assert.Throws<AggregateException>(() => runner.Dispose());
            Assert.That(trace, Does.Contain("A.release").And.Contain("hide"));
            Assert.That(runner.State, Is.EqualTo(LifecycleState.Disposed));
            Assert.DoesNotThrow(runner.Dispose);
        }

        [Test]
        public void SpawnRequestExecutesTheExistingTypedFactoryOnlyWhileActive()
        {
            using IObjectResolver container = Build();
            LifecycleEventHub events = container.Resolve<LifecycleEventHub>();
            int spawned = 0;
            Func<int, int> factory = amount => spawned += amount;
            using LifecycleRunner runner = new(Array.Empty<object>(), events, null, null, () => { });
            events.Spawn.OnNext(new(() => factory(3)));
            Assert.That(spawned, Is.Zero);
            runner.Run(new(container));
            events.Spawn.OnNext(new(() => factory(3)));
            events.Release.OnNext(new());
            events.Spawn.OnNext(new(() => factory(5)));
            Assert.That(spawned, Is.EqualTo(3));
        }

        [Test]
        public void DisposeEndsLiveUsageWithoutRequestingAnotherDestroy()
        {
            using IObjectResolver container = Build();
            int destroyed = 0;
            Probe probe = new("A", new());
            LifecycleRunner runner = new(new[] { probe }, container.Resolve<LifecycleEventHub>(), null, null, () => destroyed++);
            runner.Run(new(container));
            Assert.Throws<InvalidOperationException>(() => runner.Run(new(container)));
            runner.Dispose(); runner.Dispose();
            Assert.That(probe.ReleasedWithClosedUsage, Is.True);
            Assert.That(destroyed, Is.Zero);
        }

        [Test]
        public void OwnedChildrenEndBeforeParentSubscriptionsAndReleaseHooks()
        {
            using IObjectResolver container = Build();
            List<string> trace = new();
            Probe parent = new("parent", trace);
            using LifecycleRunner runner = new(new[] { parent }, container.Resolve<LifecycleEventHub>(), null, null,
                () => { }, () =>
                {
                    Assert.That(parent.ReleasedWithClosedUsage, Is.False);
                    trace.Add("children.end");
                });
            runner.Run(new(container));
            runner.Dispose();
            Assert.That(trace.IndexOf("children.end"), Is.LessThan(trace.IndexOf("parent.release")));
        }

        [Test]
        public void TargetSnapshotIsIndependentOfChangesToTheSourceList()
        {
            using IObjectResolver container = Build();
            List<string> trace = new();
            List<object> targets = new() { new Probe("A", trace) };
            using LifecycleRunner runner = new(targets, container.Resolve<LifecycleEventHub>(), null, null, () => { });
            targets.Clear();
            runner.Run(new(container));
            Assert.That(trace, Does.Contain("A.activate"));
        }

        [Test]
        public void UsageDisposalIsPermanentAndLateRegistrationDisposesImmediately()
        {
            UsageLifetime oldUsage = new();
            int disposed = 0;
            Disposable.Create(() => disposed++).AddTo(oldUsage);
            var token = oldUsage.Token;
            oldUsage.Dispose(); oldUsage.Dispose();
            Disposable.Create(() => disposed++).AddTo(oldUsage);
            using UsageLifetime nextUsage = new();
            Assert.That(disposed, Is.EqualTo(2));
            Assert.That(token.IsCancellationRequested, Is.True);
            Assert.That(oldUsage.Token.IsCancellationRequested, Is.True);
            Assert.That(nextUsage.IsDisposed, Is.False);
        }

        [Test]
        public void FailingResourceDoesNotPreventOtherDisposalsOrCancellation()
        {
            UsageLifetime lifetime = new();
            int disposed = 0;
            Disposable.Create(() => throw new InvalidOperationException()).AddTo(lifetime);
            Disposable.Create(() => disposed++).AddTo(lifetime);
            lifetime.Token.Register(() => throw new ArgumentException());
            Assert.Throws<AggregateException>(lifetime.Dispose);
            Assert.That(disposed, Is.EqualTo(1));
            Assert.That(lifetime.Token.IsCancellationRequested, Is.True);
            Assert.DoesNotThrow(lifetime.Dispose);
        }

        [Test]
        public void StartupSpawnRequestsAreAcceptedOnlyAfterAllSubscriptionsAreReady()
        {
            using IObjectResolver container = Build();
            LifecycleEventHub events = container.Resolve<LifecycleEventHub>();
            List<string> spawned = new();
            Probe sender = new("sender", new()) { Action = phase => events.Spawn.OnNext(new(() => spawned.Add(phase))) };
            using LifecycleRunner runner = new(new[] { sender }, events, () => spawned.Add("shown"), null, () => { });
            runner.Run(new(container));
            Assert.That(spawned, Is.EqualTo(new[] { "activate", "shown" }));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FailedSpawnFromSubscribedCancelsTheOwnerBeforeShow(bool useInjectedHub)
        {
            using IObjectResolver container = Build();
            LifecycleEventHub management = container.Resolve<LifecycleEventHub>();
            List<string> trace = new();
            Probe first = new("A", trace), second = new("B", trace);
            var failure = new InvalidOperationException("factory failed");
            first.Action = phase =>
            {
                if (phase == "activate")
                    (useInjectedHub ? first.Events : management).Spawn.OnNext(new(() => throw failure));
            };
            using LifecycleRunner runner = new(new[] { first, second }, management,
                () => trace.Add("show"), null, () => trace.Add("destroy"));
            Assert.That(Assert.Throws<InvalidOperationException>(() => runner.Run(new(container))), Is.SameAs(failure));
            Assert.That(trace, Does.Not.Contain("B.activate"));
            Assert.That(trace, Does.Not.Contain("show"));
            Assert.That(trace, Does.Contain("destroy"));
        }

        [Test]
        public void ReleaseCancelsUnfiredViewCallbacksAndDoesNotReplayThemOnReuse()
        {
            using IObjectResolver container = Build();
            LifecycleEventHub events = container.Resolve<LifecycleEventHub>();
            List<string> trace = new();
            Probe first = new("A", trace), second = new("B", trace);
            first.Action = phase => { if (phase == "updated") events.Release.OnNext(new()); };
            using LifecycleRunner runner = new(new[] { first, second }, events, () => trace.Add("show"), null,
                () => trace.Add("destroy"), returnToPool: () => trace.Add("pool"));
            runner.Run(new(container));
            Assert.That(trace, Does.Not.Contain("B.updated"));
            Assert.That(trace, Does.Not.Contain("show"));
            Assert.That(runner.State, Is.EqualTo(LifecycleState.Released));

            int lateValues = 0, lateCompletions = 0;
            using var old = second.LastView.Subscribe(_ => lateValues++, _ => lateCompletions++);
            Assert.That(lateCompletions, Is.EqualTo(1));
            first.Action = null;
            runner.Run(new(container));
            Assert.That(trace.FindAll(value => value == "A.initialize").Count, Is.EqualTo(2),
                "返却時の再Injectionでは初期化しない");
            Assert.That(trace.FindAll(value => value == "B.updated").Count, Is.EqualTo(1));
            Assert.That(lateValues, Is.Zero);
        }

        sealed class Probe : IReactiveView, IReleasable
        {
            readonly string name;
            readonly List<string> trace;
            UsageLifetime usage;
            internal readonly Subject<int> Signal = new();
            internal Action<string> Action;
            internal int Received;
            internal bool ReleasedWithClosedUsage;
            internal Probe(string name, List<string> trace) { this.name = name; this.trace = trace; }
            void Record(string phase) { trace.Add(name + "." + phase); Action?.Invoke(phase); }
            internal Observable<Unit> LastView;
            internal LifecycleEventHub Events;
            public void Injection(InjectableResolver resolver)
            {
                resolver.Inject(out Events);
                Record("inject");
                resolver.Injected.SubscribeLifecycle(() => Record("initialize"), resolver.Fail);
            }
            public void UpdateView(ViewContext context)
            {
                Record("view");
                LastView = context.UpdatedView;
                LastView.SubscribeLifecycle(() => Record("updated"), context.Fail);
            }
            public void Subscribe(SubscriptionContext context)
            {
                usage = context.Lifetime;
                Signal.Subscribe(amount => Received += amount).AddTo(usage);
                Record("subscribe");
                context.Subscribed.SubscribeLifecycle(() => Record("activate"), context.Fail);
            }
            public void Release() { ReleasedWithClosedUsage = usage == null || usage.IsDisposed; Record("release"); }
        }
    }
}
