using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
using DIVFactor.Event;
using DIVFactor.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// スキルが終了したことを宣言するイベントメッセージ
    /// </summary>
    public record SkillEndEvent : EventObject;

    /// <summary>
    /// MPが変化したことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Delta">変化量</param>
    public record CostUpdateRequestEvent(int Delta) : RequestEvent;
    public record CostUpdateResponseEvent(int Current) : ResponseEvent;
    public record HPUpdateRequestEvent(int Delta) : RequestEvent;
    public record HPUpdateResponseEvent(int Current) : ResponseEvent;
    public record ShieldUpdateRequestEvent(int Delta) : RequestEvent;
    public record ShieldUpdateResponseEvent(int Current) : ResponseEvent;
    public record DisableUpdateRequestEvent(int Delta) : RequestEvent;
    public record DisableUpdateResponseEvent(int Current) : ResponseEvent;

    [EventHub]
    public record ResourceUpdateEventHub(
        EventExchanger<CostUpdateRequestEvent, CostUpdateResponseEvent> Cost,
        EventExchanger<HPUpdateRequestEvent, HPUpdateResponseEvent> HP,
        EventExchanger<ShieldUpdateRequestEvent, ShieldUpdateResponseEvent> Shield,
        EventExchanger<DisableUpdateRequestEvent, DisableUpdateResponseEvent> Disable
    );

    /// <summary>
    /// ロックオンを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Selector"></param>
    public record LockOnRequestEvent(Func<IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>> Selector) : RequestEvent;
    /// <summary>
    /// ロックオンの結果を渡すためのイベントメッセージ
    /// </summary>
    /// <param name="Targets"></param>
    public record LockOnResponseEvent(IEnumerable<IEntityInteraction> Targets) : ResponseEvent;
    /// <summary>
    /// ロックオンを行うためのイベントオブジェクト
    /// </summary>
    [EventHub]
    public class LockOnEventHub : EventExchanger<LockOnRequestEvent, LockOnResponseEvent> { }

    [EventHub]
    public record InteractionEventHub(
        ResourceUpdateEventHub ResourceUpdate,
        EventPort<LockOnRequestEvent> LockOn,
        EventPort<SkillEndEvent> SkillEnd
    );

    public class CommandEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEventHub<LockOnEventHub>();
            builder.RegisterEventHub<InteractionEventHub>();
        }
    }
}
