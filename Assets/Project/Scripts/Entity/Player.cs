using DivFacter.Injectable;

/// <summary>
/// プレイヤーのMonoBehaviour
/// </summary>
public class Player : Entity, IInjectable
{
    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out PreStart);
        resolver.Inject(out AutoIncrease);
        resolver.Inject(out DeckGet);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
