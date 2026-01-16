using DivFacter.Event;
using VContainer.Unity;

namespace DivFacter.Entrypoint
{
    public record LateFixedUpdateEvent : EventObject;
    public class LateFixedUpdatable : IPostFixedTickable
    {
        EventBus<LateFixedUpdateEvent> LateFixedUpdateEvent { get; init; }
        public LateFixedUpdatable(EventBus<LateFixedUpdateEvent> lateFixedUpdateEvent)
        {
            LateFixedUpdateEvent = lateFixedUpdateEvent;
        }
        public void PostFixedTick()
        {
            LateFixedUpdateEvent.Publish(new());
        }
    }
}