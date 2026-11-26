using DIVFactor.Lifecycle;
using DIVFactor.Pooling;
using System;
using VContainer;

namespace DIVFactor.Spawner
{
    public sealed class ActionFactory : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal ActionFactory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public void Spawn() => acquire(LifetimeArguments.Create());
    }

    public sealed class Factory<TResult> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal Factory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public TResult Spawn() => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create()));
    }

    public sealed class ActionFactory<T1> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal ActionFactory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public void Spawn(T1 argument1) => acquire(LifetimeArguments.Create(argument1));
    }

    public sealed class Factory<T1, TResult> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal Factory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public TResult Spawn(T1 argument1) => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create(argument1)));
    }

    public sealed class ActionFactory<T1, T2> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal ActionFactory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public void Spawn(T1 argument1, T2 argument2) => acquire(LifetimeArguments.Create(argument1, argument2));
    }

    public sealed class Factory<T1, T2, TResult> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal Factory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public TResult Spawn(T1 argument1, T2 argument2) => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create(argument1, argument2)));
    }

    public sealed class ActionFactory<T1, T2, T3> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal ActionFactory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public void Spawn(T1 argument1, T2 argument2, T3 argument3) => acquire(LifetimeArguments.Create(argument1, argument2, argument3));
    }

    public sealed class Factory<T1, T2, T3, TResult> : IMoldFactory
    {
        readonly Func<LifetimeArguments, IObjectResolver> acquire;
        internal Factory(Func<LifetimeArguments, IObjectResolver> acquire) => this.acquire = acquire;
        public TResult Spawn(T1 argument1, T2 argument2, T3 argument3) => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create(argument1, argument2, argument3)));
    }

    internal static class FactoryResult
    {
        internal static TResult Resolve<TResult>(IObjectResolver container)
        {
            try { return container.Resolve<TResult>(); }
            catch
            {
                container.Resolve<LifecycleEventHub>().Destroy.OnNext(new());
                throw;
            }
        }
    }
}
