using DivFacter.Event;
using VContainer.Unity;

namespace DivFacter.Entrypoint
{
    public record PreUpdateEvent : EventObject;
    public class PreUpdatable : ITickable
    {
        EventBus<PreUpdateEvent> PreUpdateEvent { get; init; }
        public PreUpdatable(EventBus<PreUpdateEvent> preUpdateEvent)
        {
            PreUpdateEvent = preUpdateEvent;
        }
        public void Tick()
        {
            PreUpdateEvent.Publish(new());
        }
    }
}
