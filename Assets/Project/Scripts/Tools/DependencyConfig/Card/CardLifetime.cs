using DIVFactor.Lifetime;

namespace DConfig.CardLife
{
    public class CardLifetime : LifetimeObject
    {
        protected override void Install(ContainerInstaller builder)
        {

        }

        protected override void Register(ComponentRegister register)
        {
            register.ComponentInChild<CardBase>();
            register.ComponentInChild<CardSelector>();
            register.ComponentInChild<CardDesign>();
        }
    }
}