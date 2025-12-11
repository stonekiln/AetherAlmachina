using UnityEngine;
using DivFacter.Event;

namespace DConfig.Battle.Event
{
    public record CardActivateEventBundle(EventBus<CardSelectEvent> Select, EventBus<CardCancelEvent> Cancel, EventBus<CardInvokeEvent> Invoke);
    public record CardCreateEventBundle(EventBus<CardCreateRequestEvent> Request, EventBus<CardCreateResponseEvent> Response);

    public record CardSelectEvent(int Index) : EventObject;
    public record CardCancelEvent(int Index) : EventObject;
    public record CardInvokeEvent : EventObject;
    public record CardCreateRequestEvent(SkillData Data) : EventObject;
    public record CardCreateResponseEvent(GameObject GameObject) : EventObject;
}