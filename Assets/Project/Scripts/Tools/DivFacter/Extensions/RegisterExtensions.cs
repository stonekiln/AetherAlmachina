using System.Collections.Generic;
using System.Linq;
using DivFacter.Binder;
using DivFacter.Injectable;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DivFacter.Extensions
{
    public static class RegisterExtensions
    {
        /// <summary>
        /// IInjectableを実装したMonoBehaviourに対してインジェクトを予約する
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="injectables">インジェクトする対象</param>
        public static void ReserveInjection(this IContainerBuilder builder, IEnumerable<IInjectable> injectables)
        {
            builder.RegisterBuildCallback(container =>
            {
                foreach (IInjectable injectable in injectables)
                {
                    injectable.InjectDependencies(new(container));
                }
            });
        }
        public static void RegisterBinderInHierarchy<T>(this IContainerBuilder builder) where T : MonoBehaviour, IObjectBinderBase
        {
            builder.RegisterComponentInHierarchy<T>().AsImplementedInterfaces();
        }
        public static void ReserveBinding(this IContainerBuilder builder, IEnumerable<IObjectBinderBase> binderBases)
        {
            builder.RegisterBuildCallback(container =>
            {
                foreach (IObjectBinderBase binderBase in binderBases)
                {
                    binderBase.Reserve(new(container));
                }
            });
        }
    }
}