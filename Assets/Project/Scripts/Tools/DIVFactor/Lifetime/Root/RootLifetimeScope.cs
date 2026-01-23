using System.Linq;
using DIVFactor.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime.Root
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            foreach (Component spawner in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Where(mono => mono is ILifetimeSpawner))
            {
                ((ILifetimeSpawner)spawner).SpawnConfigure(new(spawner.transform, null));
            }
        }
    }
}