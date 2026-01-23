using DConfig.EntityLife.Installer;
using DIVFactor.Lifetime;

namespace DConfig.PlayerLife
{
    public class PlayerLifetime : LifetimeObject
    {
        protected override void Install(ContainerInstaller installer)
        {
            installer.Install<CardEventInstaller>();
            installer.Install<DeckEventInstaller>();
        }

        protected override void Register(ComponentRegister register)
        {
            register.ComponentInChild<Player>();
            register.ComponentInChild<HandVisualizer>();
            register.ComponentInChild<CostDisplay>();
        }
    }
}
