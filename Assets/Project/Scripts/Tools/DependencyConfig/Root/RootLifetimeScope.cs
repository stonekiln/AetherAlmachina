using System.Linq;
using DivFacter.Entrypoint;
using DivFacter.EntryPoint;
using DivFacter.Event;
using DivFacter.Lifetime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<EventBus<InitializeEvent>>(Lifetime.Scoped);
        builder.RegisterEntryPoint<Initializable>(Lifetime.Scoped);
        builder.Register<EventBus<PreStartEvent>>(Lifetime.Scoped);
        builder.RegisterEntryPoint<PreStartable>(Lifetime.Scoped);
        builder.Register<EventBus<PostStartEvent>>(Lifetime.Scoped);
        builder.RegisterEntryPoint<PostStartable>(Lifetime.Scoped);
        builder.Register<EventBus<PreUpdateEvent>>(Lifetime.Singleton);
        builder.RegisterEntryPoint<PreUpdatable>(Lifetime.Singleton);
        builder.Register<EventBus<PreFixedUpdateEvent>>(Lifetime.Singleton);
        builder.RegisterEntryPoint<PreFixedUpdatable>(Lifetime.Singleton);
        builder.Register<EventBus<LateFixedUpdateEvent>>(Lifetime.Singleton);
        builder.RegisterEntryPoint<LateFixedUpdatable>(Lifetime.Singleton);

        foreach (Component spawner in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Where(mono => mono is ILifetimeSpawner))
        {
            ((ILifetimeSpawner)spawner).SpawnConfigure(new(spawner.transform, null));
        }
    }
}
