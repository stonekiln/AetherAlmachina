using System;
using DIVFactor.Lifetime;
using DIVFactor.Pooling;
using UnityEngine;

namespace DIVFactor.Spawner
{
    public class SpawnerBuilder
    {
        readonly Transform spawnerTransform;
        readonly LifetimeObject parent;

        public SpawnerBuilder(Transform transform, LifetimeObject scope)
        {
            spawnerTransform = transform;
            parent = scope;
        }

        public SpawnerInjector<TScope> Register<TScope>(GameObject prefabObject) where TScope : LifetimeObject
        {
            Validate(prefabObject);
            return new(() => arguments =>
            {
                TScope scope = CreateScope<TScope>(prefabObject, arguments, null);
                try { scope.BeginUse(arguments); return scope.Container; }
                catch { scope.DestroyForOwner(); throw; }
            });
        }

        public SpawnerInjector<TScope> RegisterPooled<TScope>(GameObject prefabObject, int maxRetained = 64) where TScope : LifetimeObject
        {
            Validate(prefabObject);
            if (!parent) throw new InvalidOperationException("プールは所有するLifetimeObjectのSpawnConfigureで登録してください。");
            if (maxRetained <= 0) throw new ArgumentOutOfRangeException(nameof(maxRetained));
            return new(() =>
            {
                LifetimePool pool = spawnerTransform.gameObject.AddComponent<LifetimePool>();
                pool.Configure(parent, arguments => CreateScope<TScope>(prefabObject, arguments, pool), maxRetained);
                return pool.Rent;
            });
        }

        void Validate(GameObject prefabObject)
        {
            if (!spawnerTransform) throw new InvalidOperationException("生成先のTransformがありません。");
            if (!prefabObject) throw new ArgumentNullException(nameof(prefabObject));
        }

        TScope CreateScope<TScope>(GameObject prefabObject, LifetimeArguments arguments, LifetimePool pool) where TScope : LifetimeObject
        {
            GameObject scopeObject = new(typeof(TScope).Name);
            scopeObject.SetActive(false);
            LifetimeSeed seed = scopeObject.AddComponent<LifetimeSeed>();
            try
            {
                // 非表示のUI配下でもAwakeを実行できるよう、制御用Scopeは有効な親Scopeの下に置く。
                seed.transform.SetParent(parent ? parent.transform : spawnerTransform, false);
                seed.SpawnParent = spawnerTransform;
                seed.SetArguments(arguments);
                seed.Pool = pool;
                seed.DeferStart = true;
                seed.Create(parent, prefabObject);
                TScope scope = scopeObject.AddComponent<TScope>();
                scopeObject.SetActive(true);
                if (!scope.IsUsable) throw new InvalidOperationException($"{typeof(TScope)} のコンテナ構築に失敗しました。");
                return scope;
            }
            catch
            {
                TScope scope = scopeObject.GetComponent<TScope>();
                if (scope) scope.DestroyForOwner();
                else UnityEngine.Object.Destroy(scopeObject);
                throw;
            }
        }
    }
}
