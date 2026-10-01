using System.Collections.Generic;
using AetherAlmachina.Deck;
using AetherAlmachina.Skill;
using DIVFactor.Event;
using DIVFactor.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// カードを引く宣言をするためのイベントメッセージ
    /// </summary>
    /// <param name="Count">カードを引く枚数</param>
    public record DeckDrawRequestEvent(int Count) : RequestEvent;
    /// <summary>
    /// 引いたカードの結果を渡すためのイベントメッセージ
    /// </summary>
    /// <param name="DrawCard">引いたカード</param>
    public record DeckDrawResponseEvent(List<SkillData> DrawCard) : ResponseEvent;
    /// <summary>
    /// カードを引くためのイベントオブジェクト
    /// </summary>
    [EventHub]
    public class DeckDrawEventHub : EventExchanger<DeckDrawRequestEvent, DeckDrawResponseEvent> { }
    /// <summary>
    /// デッキをDeckManagerに渡すためのイベントメッセージ
    /// </summary>
    /// <param name="Deck">デッキの情報</param>
    public record DeckGetEvent(List<SkillData> Deck) : EventObject;

    public class DeckEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<DeckGetEvent>();
            builder.RegisterEventHub<DeckDrawEventHub>();
            builder.Register<DeckController>(Lifetime.Singleton);
        }
    }
}
