using DConfig.EntityLife.Installer;
using DivFacter.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.EnemyLife
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CardEventInstaller().Install(builder);
            new DeckEventInstaller().Install(builder);

            builder.RegisterComponentInHierarchy<Enemy>().UnderTransform(transform);

            builder.ReserveInjection(transform.FindInjectable());
            builder.ReserveBinding(Parent.Parent.transform.FindBinder());
        }
    }
}
