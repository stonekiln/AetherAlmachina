using DConfig.EntityLife.Installer;
using DivFacter.Extensions;
using DivFacter.Lifetime;
using VContainer;
using VContainer.Unity;

namespace DConfig.PlayerLife
{
    public class PlayerLifetime : LifetimeObject
    {
        protected override void Install(InstallBuilder builder)
        {
            builder.Install<CardEventInstaller>();
            builder.Install<DeckEventInstaller>();
        }

        protected override void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<Player>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<HandVisualizer>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CostDisplay>().UnderTransform(transform);

            builder.ReserveBinding(Parent.transform.FindBinder());
        }
    }
}
