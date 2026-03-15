using DIVFactor.Injectable;
using UnityEngine;

public class UIChildMark : MonoBehaviour, IInjectable
{
    public void Injection(InjectableResolver resolver)
    {
        resolver.Bind(this);
    }
}