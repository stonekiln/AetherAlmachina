using DIVFactor.Injectable;
using UnityEngine;

/// <summary>
/// エネミーのMonoBehaviour
/// </summary>
public class Enemy : Entity
{
    Player playerObject;
    BrainBase brain;

    public override void InjectDependencies(InjectableResolver resolver)
    {
        base.InjectDependencies(resolver);
    }

    // Update is called once per frame
    void Update()
    {

    }


}
