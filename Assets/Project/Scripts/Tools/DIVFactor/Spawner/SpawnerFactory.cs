using System;
using DIVFactor.Lifetime;
using VContainer;

namespace DIVFactor.Spawner
{
    public class SpawnerFactory<TScope> where TScope : LifetimeObject
    {
        Func<LifetimeSeed> MakeSeed { get; init; }
        Func<LifetimeSeed, IObjectResolver> GetResolver { get; init; }
        public SpawnerFactory(Func<LifetimeSeed> makeSeed, Func<LifetimeSeed, IObjectResolver> makeResolver)
        {
            MakeSeed = makeSeed;
            GetResolver = makeResolver;
        }
        public void Inject(out Action action)
        {
            action = () => GetResolver(MakeSeed());
        }
        public void Inject<TResult>(out Func<TResult> factory)
        {
            factory = () => GetResolver(MakeSeed()).Resolve<TResult>();
        }
        public void Inject<TParam>(out Action<TParam> action)
        {
            action = param => GetResolver(MakeSeed().SetCallback(builder => builder.RegisterInstance(param)));
        }
        public void Inject<TParam, TResult>(out Func<TParam, TResult> factory)
        {
            factory = param => GetResolver(MakeSeed().SetCallback(builder => builder.RegisterInstance(param))).Resolve<TResult>();
        }
        public void Inject<TParam1, TParam2>(out Action<TParam1, TParam2> action)
        {
            action = (param1, param2) => GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
            }));
        }
        public void Inject<TParam1, TParam2, TResult>(out Func<TParam1, TParam2, TResult> factory)
        {
            factory = (param1, param2) =>
            GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
            })).Resolve<TResult>();
        }
        public void Inject<TParam1, TParam2, TParam3>(out Action<TParam1, TParam2, TParam3> action)
        {
            action = (param1, param2, param3) => GetResolver(MakeSeed().SetCallback(builder =>
            {
                builder.RegisterInstance(param1);
                builder.RegisterInstance(param2);
                builder.RegisterInstance(param3);
            }));
        }
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