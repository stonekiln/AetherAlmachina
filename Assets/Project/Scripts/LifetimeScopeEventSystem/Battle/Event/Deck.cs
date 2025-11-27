using System.Collections.Generic;
using R3;
using UnityEngine;
using Utility;

namespace LSES.Battle.Event
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
    public record DeckDrawEventBundle(EventBus<DeckDrawRequestEvent> Request, EventBus<DeckDrawResponseEvent> Response);

    public record DeckDrawRequestEvent(int Count) : EventObject;
    public record DeckDrawResponseEvent(List<SkillData> DrawCard) : EventObject;
    public record DeckGetEvent(DeckList List) : EventObject;
}