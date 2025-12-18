using DivFacter.Injectable;
using UnityEngine;

/// <summary>
/// エネミーのMonoBehaviour
/// </summary>
public class Enemy : Entity, IInjectable
{
    [SerializeField] GameObject playerObject;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out PreStart);
        resolver.Inject(out AutoIncrease);
        resolver.Inject(out DeckGet);
        target = playerObject.GetComponent<Player>();
        Encount();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Encount()
    {
        playerObject.GetComponent<Player>().target = this;
    }
}
