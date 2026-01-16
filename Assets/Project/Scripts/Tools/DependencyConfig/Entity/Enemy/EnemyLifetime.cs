using DConfig.EntityLife.Installer;
using DivFacter;
using DivFacter.Extensions;
using DivFacter.Lifetime;
using VContainer;
using VContainer.Unity;

namespace DConfig.EnemyLife
{
    public class EnemyLifetime : LifetimeObject
    {
        protected override void Install(InstallBuilder builder)
        {
            builder.Install<CardEventInstaller>();
            builder.Install<DeckEventInstaller>();
        }

        protected override void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<Enemy>().UnderTransform(transform);

            builder.ReserveBinding(Parent.Parent.transform.FindBinder());
        }
    }
}
