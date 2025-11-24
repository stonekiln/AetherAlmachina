namespace LSES.Battle.Event
{
    public class CardActiveEventBundle
    {
        public EventBus<CardSelectEvent> Select { get; }
        public EventBus<CardCancelEvent> Cancel { get; }
        public EventBus<CardInvokeEvent> Invoke { get; }
        public CardActiveEventBundle(EventBus<CardSelectEvent> select, EventBus<CardCancelEvent> cancel, EventBus<CardInvokeEvent> invoke)
        {
            Select = select;
            Cancel = cancel;
            Invoke = invoke;
        }
    }
    
    public record CardSelectEvent(int Index) : EventObject;
    public record CardCancelEvent(int Index) : EventObject;
    public record CardInvokeEvent : EventObject;
}