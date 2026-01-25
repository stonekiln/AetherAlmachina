using DIVFactor.Binder;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime
{
    /// <summary>
    /// コンテナをインストールするためのクラス
    /// </summary>
    public class ContainerInstaller
    {
        IContainerBuilder Builder { get; init; }
        public ContainerInstaller(IContainerBuilder builder)
        {
            Builder = builder;
        }
        /// <summary>
        /// IInstallerが実装されたコンテナクラスをインストールする
        /// </summary>
        /// <typeparam name="T">インストールする対象となるコンテナクラスの種類</typeparam>
        public void Install<T>() where T : IInstaller, new()
        {
            new T().Install(Builder);
        }
    }

    /// <summary>
    /// ヒエラルキー上のインスタンスをDI登録するためのクラス
    /// </summary>
    public class ComponentRegister
    {
        IContainerBuilder Builder { get; init; }
        Transform LifetimeTransform { get; init; }
        public ComponentRegister(IContainerBuilder builder, Transform transform)
        {
            Builder = builder;
            LifetimeTransform = transform;
        }
        /// <summary>
        /// そのLifetimeObjectの配下にあるComponentを探してDI登録する
        /// </summary>
        /// <typeparam name="T">MonoBehaviourの種類</typeparam>
        public void ComponentInChild<T>() where T : MonoBehaviour
        {
            Builder.RegisterComponentInHierarchy<T>().UnderTransform(LifetimeTransform);
        }
        /// <summary>
        /// そのLifetimeObjectの配下にあるBinderを探してDI登録する
        /// </summary>
        /// <typeparam name="T">Binderの種類</typeparam>
        public void BinderInChild<T>() where T : MonoBehaviour, IObjectBinderBase
        {
            Builder.RegisterComponentInHierarchy<T>().UnderTransform(LifetimeTransform).AsImplementedInterfaces();
        }
    }
}