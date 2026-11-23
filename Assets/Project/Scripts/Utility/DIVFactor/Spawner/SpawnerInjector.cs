using System;
using DIVFactor.Lifecycle;
using DIVFactor.Lifetime;
using DIVFactor.Pooling;
using VContainer;

namespace DIVFactor.Spawner
{
    /// <summary>型付きファクトリーの呼び出し方を維持し、取得方法を生成またはプールへ委譲する。</summary>
    public class SpawnerInjector<TScope> where TScope : LifetimeObject
    {
        readonly Func<Func<LifetimeArguments, IObjectResolver>> bind;

        internal SpawnerInjector(Func<Func<LifetimeArguments, IObjectResolver>> bind) => this.bind = bind;

        public SpawnerInjector(Func<LifetimeSeed> makeSeed, Func<LifetimeSeed, IObjectResolver> makeResolver)
        {
            if (makeSeed == null) throw new ArgumentNullException(nameof(makeSeed));
            if (makeResolver == null) throw new ArgumentNullException(nameof(makeResolver));
            bind = () => arguments => makeResolver(makeSeed().SetArguments(arguments));
        }

        public void Inject(out Action action)
        {
            var factory = bind();
            action = () => factory(LifetimeArguments.Create());
        }
        public void Inject<TResult>(out Func<TResult> factory)
        {
            var rent = bind();
            factory = () => ResolveResult<TResult>(rent(LifetimeArguments.Create()));
        }
        public void Inject<TParam>(out Action<TParam> action)
        {
            var factory = bind();
            action = value => factory(LifetimeArguments.Create(value));
        }
        public void Inject<TParam, TResult>(out Func<TParam, TResult> factory)
        {
            var rent = bind();
            factory = value => ResolveResult<TResult>(rent(LifetimeArguments.Create(value)));
        }
        public void Inject<T1, T2>(out Action<T1, T2> action)
        {
            var factory = bind();
            action = (first, second) => factory(LifetimeArguments.Create(first, second));
        }
        public void Inject<T1, T2, TResult>(out Func<T1, T2, TResult> factory)
        {
            var rent = bind();
            factory = (first, second) => ResolveResult<TResult>(rent(LifetimeArguments.Create(first, second)));
        }
        public void Inject<T1, T2, T3>(out Action<T1, T2, T3> action)
        {
            var factory = bind();
            action = (first, second, third) => factory(LifetimeArguments.Create(first, second, third));
        }
        public void Inject<T1, T2, T3, TResult>(out Func<T1, T2, T3, TResult> factory)
        {
            var rent = bind();
            factory = (first, second, third) => ResolveResult<TResult>(rent(LifetimeArguments.Create(first, second, third)));
        }

        static TResult ResolveResult<TResult>(IObjectResolver resolver)
        {
            try { return resolver.Resolve<TResult>(); }
            catch
            {
                // 戻り値の登録ミスでも、生成済みの個体を貸出中のまま残さない。
                resolver.Resolve<LifecycleEventHub>().Destroy.OnNext(new());
                throw;
            }
        }
    }
}
