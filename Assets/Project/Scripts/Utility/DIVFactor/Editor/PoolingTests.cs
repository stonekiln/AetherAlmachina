using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Pooling;
using NUnit.Framework;
using R3;
using System;
using System.Collections.Generic;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    public class PoolingTests
    {
        [Test]
        public void ArgumentsChangeWithoutReplacingContainerOrFixedSingleton()
        {
            LifetimeArguments arguments = LifetimeArguments.Create("first", 10);
            ContainerBuilder builder = new();
            arguments.Register(builder);
            builder.Register<FixedService>(VContainer.Lifetime.Singleton);
            using IObjectResolver container = builder.Build();
            FixedService fixedService = container.Resolve<FixedService>();
            Assert.That(container.Resolve<string>(), Is.EqualTo("first"));
            arguments.Replace(LifetimeArguments.Create<string, int>(null, 20));
            Assert.That(container.Resolve<string>(), Is.Null);
            Assert.That(container.Resolve<int>(), Is.EqualTo(20));
            Assert.That(container.Resolve<FixedService>(), Is.SameAs(fixedService));
            arguments.Clear();
            Assert.That(container.Resolve<string>(), Is.Null);
            Assert.That(container.Resolve<int>(), Is.Zero);
        }

        [Test]
        public void InvalidArgumentSignatureDoesNotPartiallyOverwritePreviousValues()
        {
            Assert.Throws<ArgumentException>(() => LifetimeArguments.Create(1, 2));
            LifetimeArguments arguments = LifetimeArguments.Create("first", 10);
            ContainerBuilder builder = new();
            arguments.Register(builder);
            using IObjectResolver container = builder.Build();
            Assert.Throws<ArgumentException>(() => arguments.Replace(LifetimeArguments.Create("second", 3f)));
            Assert.That(container.Resolve<string>(), Is.EqualTo("first"));
            Assert.That(container.Resolve<int>(), Is.EqualTo(10));
        }

        [Test]
        public void EmptyPoolCreatesThenReusesTheSameContainerWithNewArguments()
        {
            using Owner owner = new();
            Scope first = owner.Rent("first");
            IObjectResolver container = first.Container;
            FixedService fixedService = container.Resolve<FixedService>();
            first.View.Events.Release.OnNext(new());
            Assert.That(owner.Pool.InactiveCount, Is.EqualTo(1));
            Assert.That(first.View.Payload, Is.Null);
            Scope second = owner.Rent("second");
            Assert.That(second, Is.SameAs(first));
            Assert.That(second.Container, Is.SameAs(container));
            Assert.That(second.View.Payload.Name, Is.EqualTo("second"));
            Assert.That(second.View.InitializedNames, Is.EqualTo(new[] { "first", "second" }));
            Assert.That(container.Resolve<FixedService>(), Is.SameAs(fixedService));
            Assert.That(fixedService.Disposed, Is.False);
            Assert.That(owner.Created, Is.EqualTo(1));
            Assert.That(owner.Pool.ActiveCount, Is.EqualTo(1));
        }

        [Test]
        public void OldUsageEventsCannotReleaseDestroyOrSpawnInTheNextUsage()
        {
            using Owner owner = new();
            Scope scope = owner.Rent("first");
            LifecycleEventHub oldEvents = scope.View.Events;
            UsageLifetime oldUsage = scope.View.Usage;
            oldEvents.Release.OnNext(new());
            owner.Rent("second");
            int spawned = 0;
            Assert.DoesNotThrow(() =>
            {
                oldEvents.Release.OnNext(new());
                oldEvents.Destroy.OnNext(new());
                oldEvents.Spawn.OnNext(new(() => spawned++));
            });
            Assert.That(oldUsage.IsDisposed, Is.True);
            Assert.That(scope.Runner.State, Is.EqualTo(LifecycleState.Active));
            Assert.That(owner.Pool.ActiveCount, Is.EqualTo(1));
            Assert.That(spawned, Is.Zero);
            scope.View.Events.Spawn.OnNext(new(() => spawned++));
            Assert.That(spawned, Is.EqualTo(1));
        }

        [Test]
        public void RepeatedUsesDoNotAccumulateSubscriptionsAndReleaseRunsAfterUnsubscribe()
        {
            using Owner owner = new();
            Scope scope = null;
            for (int use = 0; use < 100; use++)
            {
                scope = owner.Rent(use.ToString());
                int previous = scope.View.Received;
                scope.View.Signal.OnNext(1);
                Assert.That(scope.View.Received, Is.EqualTo(previous + 1));
                scope.View.Events.Release.OnNext(new());
                scope.View.Signal.OnNext(1);
                Assert.That(scope.View.Received, Is.EqualTo(previous + 1));
                Assert.That(scope.View.ReleasedAfterUnsubscribe, Is.True);
                Assert.That(scope.View.Payload, Is.Null);
            }
            Assert.That(owner.Created, Is.EqualTo(1));
            Assert.That(scope.View.ReleaseCount, Is.EqualTo(100));
        }

        [Test]
        public void PoolCapacityDiscardsExcessReturnsWithoutLeakingActiveInstances()
        {
            using Owner owner = new(1);
            Scope first = owner.Rent("first"), second = owner.Rent("second"), third = owner.Rent("third");
            first.View.Events.Release.OnNext(new());
            second.View.Events.Release.OnNext(new());
            third.View.Events.Release.OnNext(new());
            Assert.That(owner.Pool.ActiveCount, Is.Zero);
            Assert.That(owner.Pool.InactiveCount, Is.EqualTo(1));
            Assert.That(owner.Destroyed, Is.EqualTo(2));
        }

        [Test]
        public void DisposeDestroysBothBorrowedAndWaitingInstancesAndRejectsFurtherRents()
        {
            using Owner owner = new();
            Scope waiting = owner.Rent("waiting"), active = owner.Rent("active");
            waiting.View.Events.Release.OnNext(new());
            owner.Pool.Dispose();
            Assert.That(waiting.Closed && active.Closed, Is.True);
            Assert.That(owner.Destroyed, Is.EqualTo(2));
            Assert.Throws<ObjectDisposedException>(() => owner.Rent("late"));
            Assert.DoesNotThrow(owner.Pool.Dispose);
        }

        [Test]
        public void DestroyRequestDiscardsThePhysicalInstanceInsteadOfCachingIt()
        {
            using Owner owner = new();
            Scope first = owner.Rent("first");
            first.View.Events.Destroy.OnNext(new());
            Scope second = owner.Rent("second");
            Assert.That(second, Is.Not.SameAs(first));
            Assert.That(first.Closed, Is.True);
            Assert.That(owner.Created, Is.EqualTo(2));
        }

        [Test]
        public void DestroyedWaitingEntryIsRecreatedOnTheNextRent()
        {
            using Owner owner = new();
            Scope first = owner.Rent("first");
            first.View.Events.Release.OnNext(new());
            first.Close();
            Scope second = owner.Rent("second");
            Assert.That(second, Is.Not.SameAs(first));
            Assert.That(owner.Created, Is.EqualTo(2));
            Assert.That(owner.Pool.ActiveCount, Is.EqualTo(1));
        }

        [Test]
        public void ForeignAndDuplicateReturnsDoNotMixPools()
        {
            using Owner first = new(), second = new();
            Scope instance = first.Rent("first");
            Assert.That(second.Pool.Return(instance), Is.False);
            instance.View.Events.Release.OnNext(new());
            Assert.That(first.Pool.Return(instance), Is.False);
            Assert.That(second.Pool.InactiveCount, Is.Zero);
        }

        [Test]
        public void FailedInitializationDestroysTheBrokenInstanceAndThePoolRemainsUsable()
        {
            using Owner owner = new();
            Assert.Throws<InvalidOperationException>(() => owner.Pool.Rent(LifetimeArguments.Create(new Payload("bad", FailInitialize: true))));
            Assert.That(owner.Destroyed, Is.EqualTo(1));
            Scope healthy = owner.Rent("healthy");
            Assert.That(healthy.View.Payload.Name, Is.EqualTo("healthy"));
            Assert.That(owner.Pool.ActiveCount, Is.EqualTo(1));
        }

        [Test]
        public void FailedCleanupIsNotReturnedToThePool()
        {
            using Owner owner = new();
            Scope broken = owner.Pool.Rent(LifetimeArguments.Create(new Payload("bad", FailRelease: true)));
            Assert.Throws<AggregateException>(() => broken.Runner.Dispose());
            broken.Close();
            Assert.That(broken.Closed, Is.True);
            Assert.That(owner.Rent("healthy"), Is.Not.SameAs(broken));
        }

        [Test]
        public void EndingDuringActivateDoesNotReturnAnInactiveInstanceAsAFactoryResult()
        {
            using Owner owner = new();
            Assert.Throws<InvalidOperationException>(() => owner.Pool.Rent(LifetimeArguments.Create(new Payload("ended", EndImmediately: true))));
            Assert.That(owner.Pool.ActiveCount, Is.Zero);
            Assert.That(owner.Rent("healthy").View.Payload.Name, Is.EqualTo("healthy"));
        }

        [Test]
        public void ArgumentsAreBorrowedAndDoNotDisposeCallerOwnedValues()
        {
            FixedService argument = new();
            ContainerBuilder builder = new();
            LifetimeArguments.Create(argument).Register(builder);
            using (IObjectResolver container = builder.Build())
                Assert.That(container.Resolve<FixedService>(), Is.SameAs(argument));
            Assert.That(argument.Disposed, Is.False);
        }

        [Test]
        public void DisposalDuringCreationDiscardsTheNewlyCreatedInstance()
        {
            LifetimePoolStorage<object> pool = null;
            int destroyed = 0;
            pool = new(_ => { pool.Dispose(); return new object(); }, (_, __) => { }, _ => destroyed++, _ => true);
            Assert.Throws<ObjectDisposedException>(() => pool.Rent(LifetimeArguments.Create()));
            Assert.That(destroyed, Is.EqualTo(1));
        }

        [Test]
        public void NestedRentsDoNotOverwriteEachOthersArguments()
        {
            using Owner owner = new();
            Scope nested = null;
            Scope first = owner.Pool.Rent(LifetimeArguments.Create(new Payload("outer", OnInitialize: () => nested = owner.Rent("inner"))));
            Assert.That(first.View.Payload.Name, Is.EqualTo("outer"));
            Assert.That(nested.View.Payload.Name, Is.EqualTo("inner"));
            Assert.That(owner.Pool.ActiveCount, Is.EqualTo(2));
        }

        [Test]
        public void FailedPoolReturnFallsBackToFinalDestruction()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<LifecycleEventHub>();
            using IObjectResolver container = builder.Build();
            LifecycleEventHub events = container.Resolve<LifecycleEventHub>();
            int destroyed = 0;
            using LifecycleRunner runner = new(new object[] { new EndOnActivate(events) }, events, null, null, () => destroyed++,
                returnToPool: () => throw new InvalidOperationException("return"));
            Assert.Throws<InvalidOperationException>(() => runner.Run(new InjectableResolver(container)));
            Assert.That(destroyed, Is.EqualTo(1));
        }

        public sealed class FixedService : IDisposable
        {
            public bool Disposed { get; private set; }
            public void Dispose() => Disposed = true;
        }
        public record Payload(string Name, bool FailInitialize = false, bool FailRelease = false, bool EndImmediately = false, Action OnInitialize = null);

        sealed class EndOnActivate : IUsageSubscriber
        {
            readonly LifecycleEventHub events;
            public EndOnActivate(LifecycleEventHub events) => this.events = events;
            public void Subscribe(SubscriptionContext context)
                => context.Subscribed.SubscribeLifecycle(() => events.Release.OnNext(new()), context.Fail);
        }

        sealed class Owner : IDisposable
        {
            public int Created, Destroyed;
            public readonly LifetimePoolStorage<Scope> Pool;
            public Owner(int maxRetained = 64)
            {
                Pool = new(arguments => { Created++; return new Scope(this, arguments); },
                    (scope, arguments) => scope.Begin(arguments), scope => scope.Close(), scope => !scope.Closed, maxRetained);
            }
            public Scope Rent(string name) => Pool.Rent(LifetimeArguments.Create(new Payload(name)));
            public void Dispose() => Pool.Dispose();
        }

        sealed class Scope
        {
            readonly Owner owner;
            readonly LifetimeArguments arguments;
            public readonly IObjectResolver Container;
            public readonly LifecycleRunner Runner;
            public readonly View View = new();
            public bool Closed;
            public Scope(Owner owner, LifetimeArguments arguments)
            {
                this.owner = owner; this.arguments = arguments;
                ContainerBuilder builder = new();
                arguments.Register(builder);
                builder.RegisterEventHub<LifecycleEventHub>();
                builder.Register<FixedService>(VContainer.Lifetime.Singleton);
                Container = builder.Build();
                Runner = new(new[] { View }, Container.Resolve<LifecycleEventHub>(), null, null, Close,
                    returnToPool: () => owner.Pool.Return(this), clearArguments: arguments.Clear);
            }
            public void Begin(LifetimeArguments next) { arguments.Replace(next); Runner.Run(new(Container)); }
            public void Close()
            {
                if (Closed) return;
                Closed = true;
                try { Runner.Dispose(); }
                finally { Container.Dispose(); owner.Destroyed++; owner.Pool.NotifyDestroyed(this); }
            }
        }

        sealed class View : IActiveComponent, IReleasable
        {
            public Payload Payload;
            public LifecycleEventHub Events;
            public UsageLifetime Usage;
            public readonly Subject<int> Signal = new();
            public readonly List<string> InitializedNames = new();
            public int Received, ReleaseCount;
            public bool ReleasedAfterUnsubscribe;
            public void Injection(InjectableResolver resolver)
            {
                resolver.Inject(out Payload);
                resolver.Inject(out Events);
                resolver.Injected.SubscribeLifecycle(Initialize, resolver.Fail);
            }
            public void Initialize()
            {
                if (Payload.FailInitialize) throw new InvalidOperationException("initialize");
                InitializedNames.Add(Payload.Name);
                Payload.OnInitialize?.Invoke();
            }
            public void Subscribe(SubscriptionContext context)
            {
                Usage = context.Lifetime;
                Signal.Subscribe(value => Received += value).AddTo(Usage);
                context.Subscribed.SubscribeLifecycle(Activate, context.Fail);
            }
            public void Activate() { if (Payload.EndImmediately) Events.Release.OnNext(new()); }
            public void Release()
            {
                ReleaseCount++;
                ReleasedAfterUnsubscribe = Usage == null || Usage.IsDisposed;
                if (Payload?.FailRelease == true) throw new InvalidOperationException("release");
            }
        }
    }
}
