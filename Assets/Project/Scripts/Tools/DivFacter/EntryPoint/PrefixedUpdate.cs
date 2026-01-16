using DivFacter.Event;
using VContainer.Unity;

namespace DivFacter.EntryPoint
{
    public record PreFixedUpdateEvent : EventObject;
    /// <summary>
    /// Startの直後にイベントを発行するクラス
    /// </summary>
    public class PreFixedUpdatable : IFixedTickable
    {
        EventBus<PreFixedUpdateEvent> PreFixedUpdateEvent { get; init; }
        public PreFixedUpdatable(EventBus<PreFixedUpdateEvent> preFixedUpdateEvent)
        {
            PreFixedUpdateEvent = preFixedUpdateEvent;
        }
        public void FixedTick()
        {
            PreFixedUpdateEvent.Publish(new());
        }
    }
}