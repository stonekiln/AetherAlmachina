using System;
using System.Collections.Generic;
using DConfig.EntityLife.Event;
using DIVFactor.Injectable;
using UnityEngine;

public abstract class BrainBase : MonoBehaviour, IInjectable
{
    CardActivateEventBundle CardActivate;
    Func<List<ICardData>> GetHand;
    List<ICardData> Hand => GetHand();
    public virtual void Injection(InjectableResolver resolver)
    {
        resolver.Inject(out CardActivate);
        GetHand = () => resolver.GetComponent<HandController>().Hand;
    }

    protected void Select(int index)
    {
        CardActivate.Select.OnNext(new(Hand[index], index));
    }
    protected void Cancel(int index)
    {
        CardActivate.Cancel.OnNext(new(Hand[index], index));
    }
    protected void Activate()
    {
        CardActivate.Invoke.OnNext(new());
    }
}