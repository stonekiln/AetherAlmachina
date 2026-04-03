using DIVFactor.Injectable;

namespace AetherAlmachina.Entities.Brain
{
    public class EnemyBrain : BrainBase, IInjectable
    {
        public override void Injection(InjectableResolver resolver)
        {
            base.Injection(resolver);
        }
    }
}