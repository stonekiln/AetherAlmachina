using DIVFactor.Lifecycle;
using System.Collections.Generic;
using AetherAlmachina.Skill;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
using R3;
using Tools.Extensions;

namespace AetherAlmachina.Deck
{
    /// <summary>
    /// それぞれのエンティティのデッキを管理するクラス
    /// </summary>
    public class DeckController
    {
        readonly EventPort<DeckGetEvent> DeckGet;
        readonly DeckDrawEventHub DeckDraw;
        public List<SkillData> Deck { get; private set; }

        public DeckController(EventPort<DeckGetEvent> deckGet, DeckDrawEventHub deckDraw)
        {
            DeckGet = deckGet;
            DeckDraw = deckDraw;
        }

        /// <summary>
        /// 各種イベントの購読を行う
        /// </summary>
        /// <param name="lifetime">イベントの購読期間を決定する</param>
        public void Subscribe(UsageLifetime lifetime)
        {
            DeckGet.Subscribe(log => Deck = log.Deck.Shuffle()).AddTo(lifetime);
            DeckDraw.AsObservable().Subscribe(log => new(Draw(log.Count))).AddTo(lifetime);
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