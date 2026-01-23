using DIVFactor.Lifetime;
using Unity.VisualScripting;
using UnityEngine;

namespace DIVFactor.Spawner
{
    public class SpawnerBuilder
    {
        Transform SpawnerTransform { get; init; }
        LifetimeObject Parent { get; init; }

        public SpawnerBuilder(Transform transform, LifetimeObject scope)
        {
            SpawnerTransform = transform;
            Parent = scope;
        }
        public SpawnerFactory<TScope> Register<TScope>(GameObject prefabObject) where TScope : LifetimeObject
        {
            return new(() =>
            {
                LifetimeSeed seed = new GameObject(typeof(TScope).Name).AddComponent<LifetimeSeed>();
                seed.gameObject.SetActive(false);
                seed.transform.SetParent(SpawnerTransform, false);
                seed.Create(Parent, prefabObject);
                return seed;
            },
            seed =>
            {
                TScope scope = seed.AddComponent<TScope>();
                scope.gameObject.SetActive(true);
                return scope.Container;
            });
        }
    }
}