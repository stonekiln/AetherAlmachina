using AetherAlmachina.Card;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record CardActiveEventBundle(EventBus<CardSelectEvent> Select, EventBus<CardCancelEvent> Cancel, EventBus<CardInvokeEvent> Invoke);

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
}