using System;
using DIVFactor.Lifetime;
using DIVFactor.Pooling;
using VContainer;
namespace DIVFactor.Spawner
{
    public sealed class SpawnerInjector<TScope> where TScope : LifetimeObject
    {
        readonly Func<Type, string, Func<LifetimeArguments, IObjectResolver>> bind;
        internal SpawnerInjector(Func<Type, string, Func<LifetimeArguments, IObjectResolver>> bind) => this.bind = bind;
        public void Inject(out ActionFactory factory, string fieldName = null) => factory = new(bind(typeof(ActionFactory), fieldName));
        public void Inject(out Action factory)
        {
            var acquire = bind(typeof(Action), null);
            factory = () => acquire(LifetimeArguments.Create());
        }
        public void Inject<TResult>(out Factory<TResult> factory, string fieldName = null) => factory = new(bind(typeof(Factory<TResult>), fieldName));
        public void Inject<TResult>(out Func<TResult> factory)
        {
            var acquire = bind(typeof(Func<TResult>), null);
            factory = () => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create()));
        }
        public void Inject<T1>(out ActionFactory<T1> factory, string fieldName = null) => factory = new(bind(typeof(ActionFactory<T1>), fieldName));
        public void Inject<T1>(out Action<T1> factory)
        {
            var acquire = bind(typeof(Action<T1>), null);
            factory = argument1 => acquire(LifetimeArguments.Create(argument1));
        }
        public void Inject<T1, TResult>(out Factory<T1, TResult> factory, string fieldName = null) => factory = new(bind(typeof(Factory<T1, TResult>), fieldName));
        public void Inject<T1, TResult>(out Func<T1, TResult> factory)
        {
            var acquire = bind(typeof(Func<T1, TResult>), null);
            factory = argument1 => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create(argument1)));
        }
        public void Inject<T1, T2>(out ActionFactory<T1, T2> factory, string fieldName = null) => factory = new(bind(typeof(ActionFactory<T1, T2>), fieldName));
        public void Inject<T1, T2>(out Action<T1, T2> factory)
        {
            var acquire = bind(typeof(Action<T1, T2>), null);
            factory = (argument1, argument2) => acquire(LifetimeArguments.Create(argument1, argument2));
        }
        public void Inject<T1, T2, TResult>(out Factory<T1, T2, TResult> factory, string fieldName = null) => factory = new(bind(typeof(Factory<T1, T2, TResult>), fieldName));
        public void Inject<T1, T2, TResult>(out Func<T1, T2, TResult> factory)
        {
            var acquire = bind(typeof(Func<T1, T2, TResult>), null);
            factory = (argument1, argument2) => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create(argument1, argument2)));
        }
        public void Inject<T1, T2, T3>(out ActionFactory<T1, T2, T3> factory, string fieldName = null) => factory = new(bind(typeof(ActionFactory<T1, T2, T3>), fieldName));
        public void Inject<T1, T2, T3>(out Action<T1, T2, T3> factory)
        {
            var acquire = bind(typeof(Action<T1, T2, T3>), null);
            factory = (argument1, argument2, argument3) => acquire(LifetimeArguments.Create(argument1, argument2, argument3));
        }
        public void Inject<T1, T2, T3, TResult>(out Factory<T1, T2, T3, TResult> factory, string fieldName = null) => factory = new(bind(typeof(Factory<T1, T2, T3, TResult>), fieldName));
        public void Inject<T1, T2, T3, TResult>(out Func<T1, T2, T3, TResult> factory)
        {
            var acquire = bind(typeof(Func<T1, T2, T3, TResult>), null);
            factory = (argument1, argument2, argument3) => FactoryResult.Resolve<TResult>(acquire(LifetimeArguments.Create(argument1, argument2, argument3)));
        }
    }
}
