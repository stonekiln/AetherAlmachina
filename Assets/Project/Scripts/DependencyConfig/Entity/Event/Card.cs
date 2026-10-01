using AetherAlmachina.Card;
using DIVFactor.Event;
using DIVFactor.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// カードが選択されたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Data">カードの情報</param>
    /// <param name="Index">手札のインデックス</param>
    public record CardSelectEvent(ICardData Data, int Index) : EventObject;
    /// <summary>
    /// カードの選択が解除されたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Data">カードの情報</param>
    /// <param name="Index">手札のインデックス</param>
    public record CardCancelEvent(ICardData Data, int Index) : EventObject;
    /// <summary>
    /// カードの効果を発動することを宣言するイベントメッセージ
    /// </summary>
    public record CardInvokeEvent : EventObject;
    [EventHub]
    public record CardActiveEventHub(EventPort<CardSelectEvent> Select, EventPort<CardCancelEvent> Cancel, EventPort<CardInvokeEvent> Invoke);

    public class CardEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEventHub<CardActiveEventHub>();
        }
    }
}
