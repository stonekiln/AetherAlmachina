using DConfig.StageLife.Installer;
using VContainer;
using VContainer.Unity;
using DivFacter.Extensions;
using DivFacter.Lifetime;

namespace DConfig.StageLife
{
    public class StageLifetime : LifetimeObject
    {
        protected override void Install(InstallBuilder builder)
        {
            builder.Install<CostEventInstaller>();
        }

        protected override void Register(IContainerBuilder builder)
        {
            builder.Register<DeckController>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<CostManager>();
            builder.RegisterComponentInHierarchy<EntitySpawner>();
            builder.RegisterBinderInHierarchy<PlayerUIBinder>();
        }
    }
}
