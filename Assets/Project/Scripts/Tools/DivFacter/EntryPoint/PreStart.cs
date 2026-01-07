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
        readonly EventBus<PreStartEvent> PreStart;

        public PreStartable(EventBus<PreStartEvent> preStart)
        {
            PreStart = preStart;
        }
        
        public void Start()
        {
            PreStart.Publish(new());
        }
    }
}