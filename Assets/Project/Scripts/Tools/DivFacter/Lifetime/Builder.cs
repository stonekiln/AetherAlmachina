using System;
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
        FacterLifetimeScope Parent { get; init; }

        public ObjectBuilder(FacterLifetimeScope parent)
        {
            Debug.Log(parent.name);
            Parent = parent;
        }
        public void Set<TScope, TParam, TReturn>(out Func<TParam, TReturn> spawner, GameObject prefabObject, Action<TParam, IObjectResolver> callBack)
        where TScope : FacterLifetimeScope
        where TReturn : MonoBehaviour
        {
            spawner = (param) =>
            {
                TScope scope = new GameObject(typeof(TScope).Name).AddComponent<TScope>();
                prefabObject.transform.SetParent(scope.transform);
                Debug.Log(Parent.name);
                foreach (ILifetimeSpawner spawner in scope.transform.GetComponentsInChildren<ILifetimeSpawner>())
                {
                    spawner.SpawnConfigure(new ObjectBuilder(scope));
                }
                scope = Parent.CreateChildFromPrefab(scope);
                callBack(param, scope.Container);
                return scope.Container.Resolve<TReturn>();
            };
        }
    }
}