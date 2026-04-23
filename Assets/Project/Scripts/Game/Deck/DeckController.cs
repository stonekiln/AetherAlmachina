using System.Collections.Generic;
using AetherAlmachina.Skill;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using R3;
using UnityEngine;
using Utility;

namespace AetherAlmachina.Deck
{
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

        /// <summary>
        /// 各種イベントの購読を行う
        /// </summary>
        /// <param name="monoBehaviour">イベントの購読期間を決定する</param>
        public void Subscribe(MonoBehaviour monoBehaviour)
        {
            DeckGet.Subscribe(deckData => Deck = deckData.List.Shuffle()).AddTo(monoBehaviour);
            DeckDraw.Reply(log => new(Draw(log.Count))).AddTo(monoBehaviour);
        }
        /// <summary>
        /// カードをドローする
        /// </summary>
        /// <param name="count">カードを引く枚数</param>
        /// <returns>ドローしたカード</returns>
        List<SkillData> Draw(int count)
        {
            List<SkillData> drawCards = Deck.GetRange(0, count);
            Deck = Deck.GetRange(count, Deck.Count - count);
            return drawCards;
        }
    }
}