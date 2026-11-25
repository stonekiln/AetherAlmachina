using System;
using DIVFactor.Lifetime;
using DIVFactor.Mold;
using DIVFactor.Pooling;
using UnityEngine;
using VContainer;
namespace DIVFactor.Spawner
{
    public sealed class SpawnerBuilder
    {
        readonly MonoBehaviour spawner;
        readonly Transform spawnerTransform;
        readonly LifetimeObject parent;
        readonly MoldMetadata metadata;
        public SpawnerBuilder(Transform transform, LifetimeObject scope)
        { spawnerTransform = transform; parent = scope; }
        internal SpawnerBuilder(MonoBehaviour spawner, LifetimeObject parent, MoldMetadata metadata)
        { this.spawner = spawner; spawnerTransform = spawner.transform; this.parent = parent; this.metadata = metadata; }
        public SpawnerInjector<TScope> Register<TScope>() where TScope : LifetimeObject => Declared<TScope>(false, 0);
        public SpawnerInjector<TScope> RegisterPooled<TScope>(int maxRetained = 64) where TScope : LifetimeObject => Declared<TScope>(true, maxRetained);
        // 明示Prefab版は移行用。通常のゲームコードは属性版を使う。
        public SpawnerInjector<TScope> Register<TScope>(GameObject prefab) where TScope : LifetimeObject
            => new((_, _) => Bind<TScope>(RequireMetadata(prefab), false, 0));
        public SpawnerInjector<TScope> RegisterPooled<TScope>(GameObject prefab, int maxRetained = 64) where TScope : LifetimeObject
            => new((_, _) => Bind<TScope>(RequireMetadata(prefab), true, maxRetained));
        SpawnerInjector<TScope> Declared<TScope>(bool pooled, int maxRetained) where TScope : LifetimeObject
            => new((factoryType, fieldName) =>
            {
                if (!spawner || !metadata || !parent) throw new InvalidOperationException("Spawnerの所有者・対応表・親Lifetimeがありません。");
                FactoryBinding binding = metadata.Select(spawner, factoryType, fieldName);
                return Bind<TScope>(parent.Catalog.Get(binding.Path), pooled, maxRetained);
            });
        Func<LifetimeArguments, IObjectResolver> Bind<TScope>(MoldMetadata prefab, bool pooled, int maxRetained) where TScope : LifetimeObject
        {
            if (!spawnerTransform || !prefab || !prefab.IsBaked) throw new InvalidOperationException("生成先またはMoldの事前生成情報がありません。");
            if (pooled)
            {
                if (!parent || maxRetained <= 0) throw new InvalidOperationException("Poolには有効な所有Lifetimeと保持数が必要です。");
                LifetimePool pool = spawnerTransform.gameObject.AddComponent<LifetimePool>();
                pool.Configure(parent, arguments => CreateScope<TScope>(prefab, arguments, pool), maxRetained);
                return pool.Rent;
            }
            return arguments =>
            {
                if (!parent || !parent.AcceptsSpawning)
                    throw new InvalidOperationException("所有元のLifetimeが終了しているため、生成できません。");
                TScope scope = CreateScope<TScope>(prefab, arguments, null);
                try { scope.BeginUse(arguments); return scope.Container; }
                catch { scope.DestroyForOwner(); throw; }
            };
        }
        static MoldMetadata RequireMetadata(GameObject prefab)
            => prefab && prefab.TryGetComponent(out MoldMetadata metadata) && metadata.IsBaked
                ? metadata : throw new InvalidOperationException("PrefabのMoldMetadataをEditorで生成してください。");
        TScope CreateScope<TScope>(MoldMetadata prefab, LifetimeArguments arguments, LifetimePool pool) where TScope : LifetimeObject
        {
            GameObject scopeObject = new(typeof(TScope).Name);
            TScope scope = null;
            try
            {
                scopeObject.transform.SetParent(parent ? parent.transform : spawnerTransform, false);
                // Awakeは自動Buildしない。初期情報を渡した後に明示構築する。
                scope = scopeObject.AddComponent<TScope>();
                MoldMetadata instance = UnityEngine.Object.Instantiate(prefab, scopeObject.transform, false);
                scope.Prepare(parent, instance, parent ? parent.Catalog : null, arguments, pool, spawnerTransform);
                scope.BuildPrepared();
                return scope;
            }
            catch
            {
                if (scope) scope.DestroyForOwner();
                else UnityEngine.Object.Destroy(scopeObject);
                throw;
            }
        }
    }
}
