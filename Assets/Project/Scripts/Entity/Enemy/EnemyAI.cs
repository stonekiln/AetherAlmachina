using System;
using System.Collections.Generic;
using DConfig.PalyerLife.Event;
using DivFacter.Injectable;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IInjectable
{
    CardActivateEventBundle CardActivate;
    Func<List<ICardData>> GetHand;
    List<ICardData> Hand => GetHand();
    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out CardActivate);
        resolver.Inject(out HandController handController);
        GetHand = () => handController.Hand;
    }

    void Select(int index)
    {
        CardActivate.Select.Publish(new(Hand[index], index));
    }
    void Cancel(int index)
    {
        CardActivate.Cancel.Publish(new(Hand[index], index));
    }
    void Activate()
    {
        CardActivate.Invoke.Publish(new());
    }
}