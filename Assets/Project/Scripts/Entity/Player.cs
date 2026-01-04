using DivFacter.Injectable;

/// <summary>
/// プレイヤーのMonoBehaviour
/// </summary>
public class Player : Entity
{
    public override void InjectDependencies(InjectableResolver resolver)
    {
        base.InjectDependencies(resolver);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
