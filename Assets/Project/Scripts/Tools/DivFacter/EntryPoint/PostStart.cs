using DivFacter.Event;
using VContainer.Unity;

namespace DivFacter.EntryPoint
{
    public record PostStartEvent : EventObject;
    /// <summary>
    /// Startの直後にイベントを発行するクラス
    /// </summary>
    public class PostStartable : IPostStartable
    {
        EventBus<PostStartEvent> PostStartEvent { get; init; }
        public PostStartable(EventBus<PostStartEvent> postStartEvent)
        {
            PostStartEvent = postStartEvent;
        }
        public void PostStart()
        {
            PostStartEvent.Publish(new());
        }
    }
}