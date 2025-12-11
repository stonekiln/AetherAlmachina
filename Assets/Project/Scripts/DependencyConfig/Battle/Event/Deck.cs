using System.Collections.Generic;
using DivFacter.Event;
using R3;
using UnityEngine;
using Utility;

namespace DConfig.Battle.Event
{
    public class DeckEventBundle
    {
        readonly EventBus<DeckGetEvent> DeckGet;
        readonly DeckDrawEventBundle DeckDraw;
        public List<SkillData> Deck { get; private set; }

        public DeckEventBundle(EventBus<DeckGetEvent> deckGet, DeckDrawEventBundle deckDraw)
        {
            DeckGet = deckGet;
            DeckDraw = deckDraw;
        }

        public void Subscribe(MonoBehaviour monoBehaviour)
        {
            DeckGet.Subscribe(Deck => this.Deck = Deck.List.ReadDeck().Shaffle()).AddTo(monoBehaviour);
            DeckDraw.Request.Subscribe(log => DeckDraw.Response.Publish(new(Draw(log.Count)))).AddTo(monoBehaviour);
        }
        List<SkillData> Draw(int count)
        {
            List<SkillData> drawCards = Deck.GetRange(0, count);
            Deck = Deck.GetRange(count, Deck.Count - count);
            return drawCards;
        }
    }
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
    public record DeckGetEvent(DeckList List) : EventObject;
}