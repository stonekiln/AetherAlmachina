using AetherAlmachina.Card;
using AetherAlmachina.Card.Hand;
using DConfig.EntityLife.Event;
using DIVFactor.Injectable;
using System.Collections.Generic;
using UnityEngine;

namespace AetherAlmachina.Entities.Brain
{
    public abstract class BrainBase : MonoBehaviour, IInjectable
    {
        CardActiveEventHub CardActive;
        HandController handController;
        List<ICardData> Hand => handController.Hand;
        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out CardActive);
            resolver.Inject(out handController);
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
}