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
        EventBus<InitializeEvent> InitializeEvent { get; init; }

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