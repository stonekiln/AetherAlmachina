using System.Collections.Generic;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// カードを引くためのイベントオブジェクト
    /// </summary>
    public class DeckDrawEvent : EventChannel<DeckDrawRequestEvent, DeckDrawResponseEvent> { }
    /// <summary>
    /// カードを引く宣言をするためのイベントメッセージ
    /// </summary>
    /// <param name="Count">カードを引く枚数</param>
    public record DeckDrawRequestEvent(int Count) : EventObject;
    /// <summary>
    /// 引いたカードの結果を渡すためのイベントメッセージ
    /// </summary>
    /// <param name="DrawCard">引いたカード</param>
    public record DeckDrawResponseEvent(List<SkillData> DrawCard) : EventObject;
    /// <summary>
    /// デッキをDeckManagerに渡すためのイベントメッセージ
    /// </summary>
    /// <param name="List">デッキの情報</param>
    public record DeckGetEvent(List<SkillData> List) : EventObject;
}