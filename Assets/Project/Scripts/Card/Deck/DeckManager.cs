using System.Collections.Generic;
using LSES;
using LSES.Battle.Event;
using R3;
using UnityEngine;
using Utility;
using VContainer;

public class DeckManager : MonoBehaviour
{
    public class DeckEventBundle
    {
        [Inject] EventBus<DeckGetEvent> DeckGet;
        [Inject] DeckDrawEventBundle DeckDraw;
        public List<SkillData> Deck { get; private set; }

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

    DeckEventBundle PlayerDeck;
    DeckEventBundle EnemyDeck;

    void OnEnable()
    {
        PlayerDeck.Subscribe(this);
        EnemyDeck.Subscribe(this);
    }
}