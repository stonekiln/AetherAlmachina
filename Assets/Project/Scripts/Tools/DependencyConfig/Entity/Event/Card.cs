using DivFacter.Event;

namespace DConfig.EntityLife.Event
{
    public record CardActivateEventBundle(EventBus<CardSelectEvent> Select, EventBus<CardCancelEvent> Cancel, EventBus<CardInvokeEvent> Invoke);

    public record CardSelectEvent(ICardData Data,int Index) : EventObject;
    public record CardCancelEvent(ICardData Data,int Index) : EventObject;
    public record CardInvokeEvent : EventObject;
}