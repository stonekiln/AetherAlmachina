using DIVFactor.Injectable;
using UnityEngine;

public class UIChildMark : MonoBehaviour, IInjectable
{
    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Bind(this);
    }
}