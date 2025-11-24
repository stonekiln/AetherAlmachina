using LSES.Battle.Installer;
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

            builder.RegisterComponentInHierarchy<DeckManager>();
            builder.RegisterComponentInHierarchy<HandVisualizer>();
            builder.RegisterComponentInHierarchy<CardSelecter>();
            builder.RegisterComponentInHierarchy<CostManager>();
            builder.RegisterComponentInHierarchy<Entity>();
        }
    }
}
