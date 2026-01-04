using System.Collections.Generic;
using DConfig.PlayerLife.Event;
using DivFacter.Event;
using R3;
using UnityEngine;
using Utility;

/// <summary>
/// それぞれのエンティティのデッキを管理するクラス
/// </summary>
public class DeckController
{
    readonly EventBus<DeckGetEvent> DeckGet;
    readonly DeckDrawEventBundle DeckDraw;
    public List<SkillData> Deck { get; private set; }

    public DeckController(EventBus<DeckGetEvent> deckGet, DeckDrawEventBundle deckDraw)
    {
        DeckGet = deckGet;
        DeckDraw = deckDraw;
    }

    public void Subscribe(MonoBehaviour monoBehaviour)
    {
        DeckGet.Subscribe(deckData=>Deck=deckData.List.Shaffle()).AddTo(monoBehaviour);
        DeckDraw.Request.Subscribe(log => DeckDraw.Response.Publish(new(Draw(log.Count)))).AddTo(monoBehaviour);
    }
    List<SkillData> Draw(int count)
    {
        List<SkillData> drawCards = Deck.GetRange(0, count);
        Deck = Deck.GetRange(count, Deck.Count - count);
        return drawCards;
    }
}