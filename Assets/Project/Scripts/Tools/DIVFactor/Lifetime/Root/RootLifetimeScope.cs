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
            foreach (Component component in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                if (component is ILifetimeSpawner spawner) spawner.SpawnConfigure(new(component.transform, null));
            }
        }
    }
}