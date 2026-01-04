using DConfig.BattleLife.Installer;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DivFacter.Extensions;

namespace DConfig.BattleLife
{
    public class BattleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CostEventInstaller().Install(builder);

            builder.Register<DeckController>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<CostManager>();
            builder.RegisterBinderInHierarchy<PlayerUIBinder>();
            builder.RegisterBinderInHierarchy<EntityObjectBinder>();
            
            builder.ReserveInjection(gameObject.scene.FindInjectable());
        }
    }
}
