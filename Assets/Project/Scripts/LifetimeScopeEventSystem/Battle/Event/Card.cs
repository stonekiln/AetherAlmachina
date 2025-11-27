namespace LSES.Battle.Event
{
    public record CardActivateEventBundle(EventBus<CardSelectEvent> Select, EventBus<CardCancelEvent> Cancel, EventBus<CardInvokeEvent> Invoke);

    public record CardSelectEvent(int Index) : EventObject;
    public record CardCancelEvent(int Index) : EventObject;
    public record CardInvokeEvent : EventObject;
}