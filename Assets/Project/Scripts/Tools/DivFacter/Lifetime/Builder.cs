using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DivFacter.Lifetime
{
    public class InstallBuilder
    {
        IContainerBuilder Builder { get; init; }
        public InstallBuilder(IContainerBuilder builder)
        {
            Builder = builder;
        }
        public void Install<T>() where T : IInstaller, new()
        {
            new T().Install(Builder);
        }
    }
    public class ObjectBuilder
    {
        Transform SpawnerTransform { get; init; }
        LifetimeObject Parent { get; init; }

        public ObjectBuilder(Transform transform, LifetimeObject scope)
        {
            SpawnerTransform = transform;
            Parent = scope;
        }
        public FactoryGetter<TScope> MakeSpawner<TScope>(GameObject prefabObject) where TScope : LifetimeObject
        {
            return new(() =>
            {
                LifetimeSeed seed = new GameObject(typeof(TScope).Name).AddComponent<LifetimeSeed>();
                seed.gameObject.SetActive(false);
                seed.transform.SetParent(SpawnerTransform);
                seed.Create(Parent, prefabObject);
                return seed;
            },
            seed =>
            {
                TScope scope = seed.AddComponent<TScope>();
                foreach (Component spawner in scope.transform.GetComponentsInChildren<MonoBehaviour>(true).Where(mono => mono is ILifetimeSpawner))
                {
                    ((ILifetimeSpawner)spawner).SpawnConfigure(new ObjectBuilder(spawner.transform, scope));
                }
                scope.gameObject.SetActive(true);
                scope.transform.SetParent(scope.Parent.transform);
                scope.transform.GetChild(0).SetParent(SpawnerTransform);
                return scope.Container;
            }
            );
        }
    }
    public class FactoryGetter<TScope> where TScope : LifetimeObject
    {
        Func<LifetimeSeed> MakeSeed { get; init; }
        Func<LifetimeSeed, IObjectResolver> GetResolver { get; init; }
        public FactoryGetter(Func<LifetimeSeed> makeSeed, Func<LifetimeSeed, IObjectResolver> makeResolver)
        {
            MakeSeed = makeSeed;
            GetResolver = makeResolver;
        }
        public void Get(out Action action)
        {
            action = () => GetResolver(MakeSeed());
        }
        public void Get<TResult>(out Func<TResult> factory) where TResult : MonoBehaviour
        {
            factory = () => GetResolver(MakeSeed()).Resolve<TResult>();
        }
        public void Get<TAsset>(out Action<TAsset> action, Action<TAsset, AssetRegister> callback) where TAsset : ScriptableObject
        {
            action = param => GetResolver(MakeSeed().SetCallback(register => callback(param, register)));
        }
        public void Get<TAsset, TResult>(out Func<TAsset, TResult> factory, Action<TAsset, AssetRegister> callback)
        where TResult : MonoBehaviour
        where TAsset : ScriptableObject
        {
            factory = param => GetResolver(MakeSeed().SetCallback(register => callback(param, register))).Resolve<TResult>();
        }
    }
    public class AssetRegister
    {
        IContainerBuilder Builder { get; init; }
        public AssetRegister(IContainerBuilder builder)
        {
            Builder = builder;
        }
        public void Asset<T>(T asset) where T : ScriptableObject
        {
            Builder.RegisterInstance(asset);
        }
    }
}