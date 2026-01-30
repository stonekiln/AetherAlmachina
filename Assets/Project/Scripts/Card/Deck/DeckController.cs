using System.Collections.Generic;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using R3;
using UnityEngine;
using Utility;

/// <summary>
/// それぞれのエンティティのデッキを管理するクラス
/// </summary>
public class DeckController
{
    readonly EventBus<DeckGetEvent> DeckGet;
    readonly DeckDrawEvent DeckDraw;
    public List<SkillData> Deck { get; private set; }

    public DeckController(EventBus<DeckGetEvent> deckGet, DeckDrawEvent deckDraw)
    {
        DeckGet = deckGet;
        DeckDraw = deckDraw;
    }

    public void Subscribe(MonoBehaviour monoBehaviour)
    {
        DeckGet.Subscribe(deckData => Deck = deckData.List.Shuffle()).AddTo(monoBehaviour);
        DeckDraw.Reply(log => new(Draw(log.Count))).AddTo(monoBehaviour);
    }
    List<SkillData> Draw(int count)
    {
        List<SkillData> drawCards = Deck.GetRange(0, count);
        Deck = Deck.GetRange(count, Deck.Count - count);
        return drawCards;
    }
}