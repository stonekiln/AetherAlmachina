using DivFacter.Event;
using VContainer.Unity;

namespace DivFacter.EntryPoint
{
    public record PreStartEvent : EventObject;

    /// <summary>
    /// Startの直前にイベントを発行するクラス
    /// </summary>
    public class PreStartable : IStartable
    {
        EventBus<PreStartEvent> PreStartEvent { get; init; }

        public PreStartable(EventBus<PreStartEvent> preStartEvent)
        {
            PreStartEvent = preStartEvent;
        }

        public void Start()
        {
            PreStartEvent.Publish(new());
        }
    }
}