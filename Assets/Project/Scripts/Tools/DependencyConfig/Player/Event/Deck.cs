using System.Collections.Generic;
using DivFacter.Event;

namespace DConfig.PalyerLife.Event
{
    /// <summary>
    /// カードを引くイベントをまとめたもの
    /// </summary>
    /// <param name="Request"></param>
    /// <param name="Response"></param>
    public record DeckDrawEventBundle(EventBus<DeckDrawRequestEvent> Request, EventBus<DeckDrawResponseEvent> Response);
    
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