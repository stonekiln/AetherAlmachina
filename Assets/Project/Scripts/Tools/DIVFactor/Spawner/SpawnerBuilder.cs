using DIVFactor.Lifetime;
using Unity.VisualScripting;
using UnityEngine;

namespace DIVFactor.Spawner
{
    /// <summary>
    /// スポナーの設定と生成するためのクラス
    /// </summary>
    public class SpawnerBuilder
    {
        Transform SpawnerTransform { get; init; }
        LifetimeObject Parent { get; init; }

        public SpawnerBuilder(Transform transform, LifetimeObject scope)
        {
            SpawnerTransform = transform;
            Parent = scope;
        }
        /// <summary>
        /// 生成するGameObjectとそのGameObjectが持つLifetimeObjectを設定する
        /// </summary>
        /// <typeparam name="TScope">使用するLifetimeObject</typeparam>
        /// <param name="prefabObject">生成するGameObject</param>
        /// <returns></returns>
        public SpawnerInjector<TScope> Register<TScope>(GameObject prefabObject) where TScope : LifetimeObject
        {
            return new(() =>
            {
                //LifetimeObjectを生成するための初期化情報を作る
                LifetimeSeed seed = new GameObject(typeof(TScope).Name).AddComponent<LifetimeSeed>();
                //MonoBehaviourのライフサイクルを一時停止
                seed.gameObject.SetActive(false);
                //スポナーの子に配置する
                seed.transform.SetParent(SpawnerTransform, false);
                //初期化情報を渡す
                seed.Create(Parent, prefabObject);
                return seed;
            },
            seed =>
            {
                //LifetimeSeedが付与されたMonoBehaviourに指定したLifetimeObjectのコンポーネントを追加する
                TScope scope = seed.AddComponent<TScope>();
                //自身を有効化しDIVFactorのライフサイクルに入る
                scope.gameObject.SetActive(true);
                return scope.Container;
            });
        }
    }
}