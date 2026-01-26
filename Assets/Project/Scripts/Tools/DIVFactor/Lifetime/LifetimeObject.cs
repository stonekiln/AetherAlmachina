using System;
using System.Collections.Generic;
using System.Linq;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
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
        readonly EventBus<BindEvent> BindPoint = new();
        readonly EventBus<ActiveEvent> ActivePoint = new();
        readonly EventBus<EndEvent> EndPoint = new();

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
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ChildObjects.ForEach(obj => Destroy(obj));
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
            foreach (Component component in GetComponentsInChildren<MonoBehaviour>(true))
            {
                //Injectableを探してそれにコンテナを渡す
                if (component is IInjectable injectable)
                {
                    injectable.Injection(new(container, new(BindPoint, ActivePoint, EndPoint)));
                }
                //スポナーを探してそれに自身のTransformとLifetimeScopeを渡す
                //引数に渡したインスタンスがそのスポナーの親となる
                if (component is ILifetimeSpawner spawner)
                {
                    spawner.SpawnConfigure(new SpawnerBuilder(component.transform, this));
                }
            }
            //LifetimeObjectの直下のオブジェクトは生成したGameObjectのルートである
            ChildObjects = new() { transform.GetChild(0).gameObject };
            //LifetimeObjectを親のLifetimeObjectのTransformの子に配置する
            ChildObjects[0].transform.SetParent(transform.parent, false);
            //生成したオブジェクトをスポナーの子に配置する
            transform.SetParent(Parent.transform, false);
            //EndPointを購買する
            EndPoint.Event.Take(1).Subscribe(_ => Destroy(gameObject));
            //BindPointの実行後にActivePointをフックして実行
            BindPoint.Event.Take(1).Do(onCompleted: _ =>
            {
                //ActivePointを発行する
                ActivePoint.Publish(new());
                ActivePoint.Event.OnCompleted();
            }).Subscribe();
            //BindPointを発行する
            BindPoint.Publish(new(this));
            BindPoint.Event.OnCompleted();
            //配下のコンポーネントを有効化し、本来のMonoBehaviourのライフサイクルに入る
            ChildObjects.ForEach(obj => obj.SetActive(true));
        }
        /// <summary>
        /// EndPointを発行する
        /// </summary>
        public void EntryEndPoint()
        {
            EndPoint.Publish(new());
        }
    }
}