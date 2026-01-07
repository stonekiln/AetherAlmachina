using DivFacter.Event;
using VContainer.Unity;

namespace DivFacter.EntryPoint
{
    public record InitializeEvent : EventObject;

    /// <summary>
    /// コンテナ完成直後にイベントを発行するクラス
    /// </summary>
    public class Initializable : IInitializable
    {
        readonly EventBus<InitializeEvent> InitializeEvent;

        public Initializable(EventBus<InitializeEvent> initializeEvent)
        {
            InitializeEvent = initializeEvent;
        }

        public void Initialize()
        {
            InitializeEvent.Publish(new());
        }
    }
}