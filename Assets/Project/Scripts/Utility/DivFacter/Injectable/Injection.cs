using System.Linq;
using UnityEngine;
using VContainer;

namespace DivFacter.Injectable
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IInjectableを実装したMonoBehaviourに対して依存関係を解決する
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="monoBehaviours">対象とするMonoBehaviour</param>
        public static void InjectMonoBehaviors(this IContainerBuilder builder, MonoBehaviour[] monoBehaviours)
        {
            builder.RegisterBuildCallback(container =>
                {
                    foreach (IInjectable injectable in monoBehaviours.OfType<IInjectable>())
                    {
                        injectable.InjectDependencies(new(container));
                    }
                });
        }
    }
}