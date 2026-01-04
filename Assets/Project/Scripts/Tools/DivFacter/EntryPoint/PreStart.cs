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
        readonly EventBus<PreStartEvent> Prestart;

        public PreStartable(EventBus<PreStartEvent> preStart)
        {
            Prestart = preStart;
        }
        
        public void Start()
        {
            Prestart.Publish(new());
        }
    }
}