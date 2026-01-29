using DConfig.StageLife.Installer;
using DIVFactor.Lifetime;

namespace DConfig.StageLife
{
    public class StageLifetime : LifetimeObject
    {
        protected override void Install(ContainerInstaller installer)
        {
            installer.Install<CostEventInstaller>();
        }

        protected override void Register(ComponentRegister register)
        {
            register.ComponentInChild<CostManager>();
            register.ComponentInChild<EntitySpawner>();
            register.BinderInChild<PlayerUIBinder>();
        }
    }
}
