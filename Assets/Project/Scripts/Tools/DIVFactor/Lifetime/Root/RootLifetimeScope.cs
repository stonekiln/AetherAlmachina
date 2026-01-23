using System.Linq;
using DIVFactor.Event;
using DIVFactor.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime.Root
{
    /// <summary>
    /// 全てのLifetimeObjectのルートとなる
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EventBus<ActivateEvent>>(VContainer.Lifetime.Scoped);
            foreach (Component spawner in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Where(mono => mono is ILifetimeSpawner))
            {
                ((ILifetimeSpawner)spawner).SpawnConfigure(new(spawner.transform, null));
            }
        }
    }
}