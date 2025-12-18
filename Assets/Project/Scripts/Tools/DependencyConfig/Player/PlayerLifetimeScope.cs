using DConfig.PalyerLife.Installer;
using DivFacter.EntryPoint;
using DivFacter.Event;
using VContainer;
using VContainer.Unity;

namespace DConfig.PalyerLife
{
    public class PlayerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CardEventInstaller().Install(builder);
            new DeckEventInstaller().Install(builder);
            new CardPrefabInstaller().Install(builder);

            builder.Register<EventBus<PreStartEvent>>(Lifetime.Singleton);
            builder.RegisterEntryPoint<PreStartable>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<Player>().UnderTransform(transform.parent);
            builder.RegisterComponentInHierarchy<HandVisualizer>().UnderTransform(transform.parent);
            builder.RegisterComponentInHierarchy<CostDisplay>().UnderTransform(transform.parent);
        }
    }
}
