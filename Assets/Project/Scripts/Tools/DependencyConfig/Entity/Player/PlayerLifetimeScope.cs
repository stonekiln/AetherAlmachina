using DConfig.EntityLife.Installer;
using DivFacter;
using DivFacter.Extensions;
using DivFacter.Lifetime;
using DivFacter.PlayerLife;
using VContainer;
using VContainer.Unity;

namespace DConfig.PlayerLife
{
    public class PlayerLifetimeScope : FacterLifetimeScope
    {
        protected override void Install(InstallBuilder builder)
        {
            builder.Install<CardEventInstaller>();
            builder.Install<DeckEventInstaller>();
            builder.Install<CardPrefabInstaller>();
        }

        protected override void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<Player>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<HandVisualizer>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CostDisplay>().UnderTransform(transform);
            builder.RegisterBinderInHierarchy<CardBinder>();

            builder.ReserveInjection(transform.FindInjectable());
            builder.ReserveBinding(Parent.transform.FindBinder());
            foreach (ILifetimeSpawner spawner in transform.GetComponentsInChildren<ILifetimeSpawner>())
                {
                    spawner.SpawnConfigure(new ObjectBuilder(this));
                }
        }
    }
}
