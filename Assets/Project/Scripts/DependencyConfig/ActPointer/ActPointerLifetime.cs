using AetherAlmachina.ActGauge.Pointer;
using DIVFactor.Lifetime;

namespace DConfig.ActPointerLife
{
    public class ActPointerLifetime : LifetimeObject
    {
        protected override void Install(ContainerInstaller installer)
        {

        }

        protected override void Register(ComponentRegister register)
        {
            register.ComponentInChild<PointerController>();
        }
    }
}