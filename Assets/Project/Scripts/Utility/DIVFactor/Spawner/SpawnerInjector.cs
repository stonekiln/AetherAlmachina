using System;
using DIVFactor.Lifetime;
using VContainer;

namespace DIVFactor.Spawner
{
    /// <summary>
    /// スポナー生成しフィールドにインジェクトするためのクラス
    /// </summary>
    /// <typeparam name="TScope">生成するLifetimeObject</typeparam>
    public class SpawnerInjector<TScope> where TScope : LifetimeObject
    {
        Func<LifetimeSeed> MakeSeed { get; init; }
        Func<LifetimeSeed, IObjectResolver> GetResolver { get; init; }
        public SpawnerInjector(Func<LifetimeSeed> makeSeed, Func<LifetimeSeed, IObjectResolver> makeResolver)
        {
            MakeSeed = makeSeed;
            GetResolver = makeResolver;
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータはなし
        /// </summary>
        /// <param name="action">インジェクトの対象</param>
        public void Inject(out Action action)
        {
            action = () => GetResolver(MakeSeed());
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータはなし
        /// </summary>
        /// <typeparam name="TResult">LifetimeScopeに登録したインスタンスを返す</typeparam>
        /// <param name="factory">インジェクトの対象</param>
        public void Inject<TResult>(out Func<TResult> factory)
        {
            factory = () => GetResolver(MakeSeed()).Resolve<TResult>();
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータは1個
        /// </summary>
        /// <param name="action">インジェクトの対象</param>
        public void Inject<TParam>(out Action<TParam> action)
        {
            action = param => GetResolver(MakeSeed().SetCallback(builder => builder.RegisterInstance(param)));
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータは1個
        /// </summary>
        /// <typeparam name="TResult">LifetimeScopeに登録したインスタンスを返す</typeparam>
        /// <param name="factory">インジェクトの対象</param>
        public void Inject<TParam, TResult>(out Func<TParam, TResult> factory)
        {
            factory = param => GetResolver(MakeSeed().SetCallback(builder => builder.RegisterInstance(param))).Resolve<TResult>();
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータは2個
        /// </summary>
        /// <param name="action">インジェクトの対象</param>
        public void Inject<TParam1, TParam2>(out Action<TParam1, TParam2> action)
        {
            action = (param1, param2) => GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
            }));
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータは2個
        /// </summary>
        /// <typeparam name="TResult">LifetimeScopeに登録したインスタンスを返す</typeparam>
        /// <param name="factory">インジェクトの対象</param>
        public void Inject<TParam1, TParam2, TResult>(out Func<TParam1, TParam2, TResult> factory)
        {
            factory = (param1, param2) =>
            GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
            })).Resolve<TResult>();
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータは3個
        /// </summary>
        /// <param name="action">インジェクトの対象</param>
        public void Inject<TParam1, TParam2, TParam3>(out Action<TParam1, TParam2, TParam3> action)
        {
            action = (param1, param2, param3) => GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
                builder.RegisterInstance(param3);
            }));
        }
        /// <summary>
        /// 任意のフィールドにインジェクトを行う、LifetimeObjectに登録するパラメータは3個
        /// </summary>
        /// <typeparam name="TResult">LifetimeScopeに登録したインスタンスを返す</typeparam>
        /// <param name="factory">インジェクトの対象</param>
        public void Inject<TParam1, TParam2, TParam3, TResult>(out Func<TParam1, TParam2, TParam3, TResult> factory)
        {
            factory = (param1, param2, param3) =>
            GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
                builder.RegisterInstance(param3);
            })).Resolve<TResult>();
        }
    }
}