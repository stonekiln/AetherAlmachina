using VContainer.Unity;

namespace LSES.EntryPoint
{
    public record PreStartEvent : EventObject;

    public class PreStartable : IStartable
    {
        readonly EventBus<PreStartEvent> Prestart;
        public PreStartable(EventBus<PreStartEvent> preStart)
        {
            Prestart=preStart;
        }
        public void Start()
        {
            Prestart.Publish(new());
        }
    }
}