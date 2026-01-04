using DivFacter.Injectable;
public class EnemyBrain : BrainBase,IInjectable
{
    public override void InjectDependencies(InjectableResolver resolver)
    {
        base.InjectDependencies(resolver);
    }
}