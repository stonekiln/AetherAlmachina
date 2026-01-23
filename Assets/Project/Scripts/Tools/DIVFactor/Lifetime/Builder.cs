using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime
{
    public class ContainerInstaller
    {
        IContainerBuilder Builder { get; init; }
        public ContainerInstaller(IContainerBuilder builder)
        {
            Builder = builder;
        }
        public void Install<T>() where T : IInstaller, new()
        {
            new T().Install(Builder);
        }
    }
    public class ComponentRegister
    {
        IContainerBuilder Builder { get; init; }
        Transform LifetimeTransform { get; init; }
        public ComponentRegister(IContainerBuilder builder, Transform transform)
        {
            Builder = builder;
            LifetimeTransform = transform;
        }
        public void ComponentInChild<T>() where T : MonoBehaviour
        {
            Builder.RegisterComponentInHierarchy<T>().UnderTransform(LifetimeTransform);
        }
        public void BinderInChild<T>() where T : MonoBehaviour
        {
            Builder.RegisterComponentInHierarchy<T>().UnderTransform(LifetimeTransform).AsImplementedInterfaces();
        }
    }
}