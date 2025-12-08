using LSES.Battle.Installer;
using LSES.EntryPoint;
using VContainer;
using VContainer.Unity;

namespace LSES.Battle
{
    public class BattleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CostEventInstaller().Install(builder);
            new CardEventInstaller().Install(builder);
            new DeckEventInstaller().Install(builder);
            builder.Register<EventBus<PreStartEvent>>(Lifetime.Singleton);
            builder.RegisterEntryPoint<PreStartable>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<DeckManager>();
            builder.RegisterComponentInHierarchy<CostManager>();
            builder.RegisterComponentInHierarchy<HandVisualizer>();
            builder.RegisterComponentInHierarchy<Player>();
            builder.RegisterComponentInHierarchy<Enemy>();
        }
    }
}
