using System;
using System.Collections.Generic;
using DConfig.EntityLife.Event;
using DIVFactor.Injectable;
using UnityEngine;

public abstract class BrainBase : MonoBehaviour, IInjectable
{
    CardActiveEventBundle CardActive;
    Func<List<ICardData>> GetHand;
    List<ICardData> Hand => GetHand();
    public virtual void Injection(InjectableResolver resolver)
    {
        resolver.Inject(out CardActive);
        GetHand = () => resolver.GetComponent<HandController>().Hand;
    }

    protected void Select(int index)
    {
        CardActive.Select.OnNext(new(Hand[index], index));
    }
    protected void Cancel(int index)
    {
        CardActive.Cancel.OnNext(new(Hand[index], index));
    }
    protected void Activate()
    {
        CardActive.Invoke.OnNext(new());
    }
}