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
        readonly EventBus<InitializeEvent> Prestart;

        public Initializable(EventBus<InitializeEvent> preStart)
        {
            Prestart = preStart;
        }

        public void Initialize()
        {
            Prestart.Publish(new());
        }
    }
}