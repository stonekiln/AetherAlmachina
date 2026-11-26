using DIVFactor.Mold;
using System;
using System.Linq;
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
        MoldMetadata Metadata { get; init; }
        public ComponentRegister(IContainerBuilder builder, MoldMetadata metadata)
        {
            Builder = builder;
            Metadata = metadata;
        }
        /// <summary>
        /// そのLifetimeObjectの配下にあるComponentを探してDI登録する
        /// </summary>
        /// <typeparam name="T">MonoBehaviourの種類</typeparam>
        public RegistrationBuilder ComponentInChild<T>() where T : MonoBehaviour
        {
            T target = Metadata.Components.OfType<T>().FirstOrDefault();
            if (!target) throw new InvalidOperationException(typeof(T).Name + " が事前保存一覧にありません。");
            return Builder.RegisterComponent(target);
        }
    }
}
