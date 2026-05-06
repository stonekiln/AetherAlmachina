using AetherAlmachina.Entities.Faction;
using DConfig.EntityLife.Installer;
using DIVFactor.Lifetime;

namespace DConfig.EnemyLife
{
    public class EnemyLifetime : LifetimeObject
    {
        protected override void Install(ContainerInstaller installer)
        {
            installer.Install<CardEventInstaller>();
            installer.Install<DeckEventInstaller>();
            installer.Install<TargetingEventInstaller>();
            installer.Install<CommandEventInstaller>();
        }

        protected override void Register(ComponentRegister register)
        {
            register.ComponentInChild<Enemy>();
        }
    }
}
