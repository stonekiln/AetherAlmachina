using System;
using System.Collections.Generic;
using System.Linq;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using NUnit.Framework;
using R3;
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
        public List<GameObject> ChildObjects { get; private set; }
        /// <summary>
        /// Spawnerから受け取った追加のDI登録をするためのコールバック
        /// </summary>
        Action<IContainerBuilder> Callback;
        /// <summary>
        /// LifetimeScopeがDestroyされるときに呼び出される
        /// </summary>
        EventBus<DestroyEvent> DestroyEvent;
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
        protected override void OnDestroy()
        {
            base.OnDestroy();
            DestroyEvent.Publish(new());
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
            ChildObjects = new() { transform.GetChild(0).gameObject };
            //LifetimeObjectを親のLifetimeObjectのTransformの子に配置する
            ChildObjects[0].transform.SetParent(transform.parent, false);
            //生成したオブジェクトをスポナーの子に配置する
            transform.SetParent(Parent.transform, false);
            DestroyEvent = container.Resolve<EventBus<DestroyEvent>>();
            DestroyEvent.Subscribe(_ => ChildObjects.ForEach(obj => Destroy(obj))).AddTo(this);
            //Enable直前のイベントを発行する
            EventBus<BindEvent> bind = container.Resolve<EventBus<BindEvent>>();
            bind.Event.Take(1).Do(onCompleted: _ =>
            {
                EventBus<ActivateEvent> activate = container.Resolve<EventBus<ActivateEvent>>();
                activate.Publish(new());
                activate.Event.OnCompleted();
            }).Subscribe();
            bind.Publish(new(this));
            bind.Event.OnCompleted();
            //オブジェクトを有効化し、本来のMonoBehaviourのライフサイクルに入る
            ChildObjects.ForEach(obj => obj.SetActive(true));
        }
    }
}