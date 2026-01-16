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
        Transform ParentTransform { get; init; }
        LifetimeObject Parent { get; init; }

        public ObjectBuilder(Transform transform, LifetimeObject scope)
        {
            ParentTransform = transform;
            Parent = scope;
        }
        public void Set<TScope, TParam, TReturn>(out Func<TParam, TReturn> spawner, GameObject prefabObject, Action<TParam, IObjectResolver> callBack)
        where TScope : LifetimeObject
        where TReturn : MonoBehaviour
        {
            spawner = (param) =>
            {
                LifetimeSeed seed = new GameObject(typeof(TScope).Name).AddComponent<LifetimeSeed>();
                seed.transform.SetParent(ParentTransform);
                seed.Create(Parent, prefabObject, (resolver) => callBack(param, resolver));
                TScope scope = seed.AddComponent<TScope>();
                foreach (Component spawner in scope.transform.GetComponentsInChildren<MonoBehaviour>().Where(mono => mono is ILifetimeSpawner))
                {
                    ((ILifetimeSpawner)spawner).SpawnConfigure(new ObjectBuilder(spawner.transform, scope));
                }
                scope.transform.SetParent(scope.Parent.transform);
                scope.transform.GetChild(0).SetParent(ParentTransform);
                return scope.Container.Resolve<TReturn>();
            };
        }
    }
}