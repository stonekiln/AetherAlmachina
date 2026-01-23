using System;
using System.Linq;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime
{
    /// <summary>
    /// オブジェクトにLifetimeScopeを付加する
    /// </summary>
    public abstract class LifetimeObject : LifetimeScope
    {
        /// <summary>
        /// Spawnerから受け取った追加のDI登録をするためのコールバック
        /// </summary>
        Action<IContainerBuilder> Callback;
        protected override void Awake()
        {
            //初期化情報を受け取る
            LifetimeSeed seed = gameObject.GetComponent<LifetimeSeed>();
            //ルートの場合は親がnullに設定される
            // ルートの場合は初期値でルートのLifetimeScopeが設定されているため何もしない
            if (seed.Parent) EnqueueParent(seed.Parent);
            Callback = seed.CallBack;
            //必要ないので初期化情報を削除する
            Destroy(seed);
            base.Awake();
        }
        /// <summary>
        /// ここでIInstallerが実装されたコンテナクラスをインストールする
        /// </summary>
        /// <param name="installer">インストールするためのフィールド</param>
        protected abstract void Install(ContainerInstaller installer);
        /// <summary>
        /// ここでヒエラルキー上のインスタンスをDI登録する
        /// </summary>
        /// <param name="register">DI登録するためのフィールド</param>
        protected abstract void Register(ComponentRegister register);

        protected sealed override void Configure(IContainerBuilder builder)
        {
            Install(new(builder));
            Register(new(builder, transform));
            Callback(builder);
            builder.RegisterBuildCallback(container => BuildCallback(container));
        }
        /// <summary>
        /// コンテナ完成後のコールバック処理
        /// </summary>
        /// <param name="container">完成したコンテナ</param>
        void BuildCallback(IObjectResolver container)
        {
            //配下のMonoBehaviourを全て取得する
            MonoBehaviour[] monoBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            //スポナーを探してそれに自身のTransformとLifetimeScopeを渡す
            //引数に渡したインスタンスがそのスポナーの親となる
            foreach (Component spawner in monoBehaviours.Where(mono => mono is ILifetimeSpawner))
            {
                ((ILifetimeSpawner)spawner).SpawnConfigure(new SpawnerBuilder(spawner.transform, this));
            }
            //Injectableを探してそれにコンテナを渡す
            foreach (IInjectable injectable in monoBehaviours.OfType<IInjectable>())
            {
                injectable.Injection(new(container));
            }
            //LifetimeObjectの直下のオブジェクトは生成したGameObjectのルートである
            GameObject child = transform.GetChild(0).gameObject;
            //LifetimeObjectを親のLifetimeObjectのTransformの子に配置する
            child.transform.SetParent(transform.parent, false);
            //生成したオブジェクトをスポナーの子に配置する
            transform.SetParent(Parent.transform, false);
            //Enable直前のイベントを発行する
            container.Resolve<EventBus<ActivateEvent>>().Publish(new());
            //オブジェクトを有効化し、本来のMonoBehaviourのライフサイクルに入る
            child.SetActive(true);
        }
    }
}