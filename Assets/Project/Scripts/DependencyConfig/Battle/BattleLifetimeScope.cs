using DConfig.Battle.Installer;
using DivFacter.Event;
using DivFacter.EntryPoint;
using DivFacter.Injectable;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.Battle
{
    public class BattleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CostEventInstaller().Install(builder);
            new DeckEventInstaller().Install(builder);
            new CardEventInstaller().Install(builder);
            new CardPrefabInstaller().Install(builder);

            builder.Register<EventBus<PreStartEvent>>(Lifetime.Singleton);
            builder.RegisterEntryPoint<PreStartable>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<DeckManager>();
            builder.RegisterComponentInHierarchy<CostManager>();
            builder.RegisterComponentInHierarchy<HandVisualizer>();
            builder.RegisterComponentInHierarchy<Player>();
            builder.RegisterComponentInHierarchy<Enemy>();
            builder.RegisterComponentInHierarchy<CardCreator>();
            builder.InjectMonoBehaviors(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None));
        }
    }
}
