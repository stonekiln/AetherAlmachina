using AetherAlmachina.ActGauge.Pointer;
using AetherAlmachina.Cost;
using AetherAlmachina.Stage;
using DConfig.StageLife.Installer;
using DIVFactor.Lifetime;

namespace DConfig.StageLife
{
    public class StageLifetime : LifetimeObject
    {
        protected override void Install(ContainerInstaller installer)
        {
            installer.Install<CostEventInstaller>();
            installer.Install<ActGaugeInstaller>();
        }

        protected override void Register(ComponentRegister register)
        {
            register.ComponentInChild<CostManager>();
            register.ComponentInChild<EntitySpawner>();
            register.ComponentInChild<FriendlyPointer>();
            register.ComponentInChild<HostilePointer>();
            register.BinderInChild<PlayerUIBinder>();
        }
    }
}
