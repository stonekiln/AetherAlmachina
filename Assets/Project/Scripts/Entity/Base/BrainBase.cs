using System;
using System.Collections.Generic;
using DConfig.PlayerLife.Event;
using DivFacter.Injectable;
using UnityEngine;

public abstract class BrainBase : MonoBehaviour, IInjectable
{
    CardActivateEventBundle CardActivate;
    Func<List<ICardData>> GetHand;
    List<ICardData> Hand => GetHand();
    public virtual void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out CardActivate);
        GetHand = () => resolver.GetComponent<HandController>().Hand;
    }

    protected void Select(int index)
    {
        CardActivate.Select.Publish(new(Hand[index], index));
    }
    protected void Cancel(int index)
    {
        CardActivate.Cancel.Publish(new(Hand[index], index));
    }
    protected void Activate()
    {
        CardActivate.Invoke.Publish(new());
    }
}