using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// スキルが終了したことを宣言するイベントメッセージ
    /// </summary>
    public record SkillEndEvent : EventObject;

    public record ResourceUpdateEventBundle<TReq, TRes>(EventBus<TReq> Request, EventBus<TRes> Response)
        where TReq : EventObject
        where TRes : EventObject;
    /// <summary>
    /// MPが変化したことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Delta">変化量</param>
    public record CostUpdateRequestEvent(int Delta) : EventObject;
    public record CostUpdateResponseEvent(int Current) : EventObject;
    public record HPUpdateRequestEvent(int Delta) : EventObject;
    public record HPUpdateResponseEvent(int Current) : EventObject;
    public record ShieldUpdateRequestEvent(int Delta) : EventObject;
    public record ShieldUpdateResponseEvent(int Current) : EventObject;
    public record DisableUpdateRequestEvent(int Delta) : EventObject;
    public record DisableUpdateResponseEvent(int Current) : EventObject;

    /// <summary>
    /// ロックオンを行うためのイベントオブジェクト
    /// </summary>
    public record LockOnEventBundle(EventBus<LockOnRequestEvent> Request, EventBus<LockOnResponseEvent> Response);
    /// <summary>
    /// ロックオンを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Selector"></param>
    public record LockOnRequestEvent(Func<IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>> Selector) : EventObject;
    /// <summary>
    /// ロックオンの結果を渡すためのイベントメッセージ
    /// </summary>
    /// <param name="Targets"></param>
    public record LockOnResponseEvent(IEnumerable<IEntityInteraction> Targets) : EventObject;

    public record ResourceUpdateEventBundle(
        ResourceUpdateEventBundle<CostUpdateRequestEvent, CostUpdateResponseEvent> Cost,
        ResourceUpdateEventBundle<HPUpdateRequestEvent, HPUpdateResponseEvent> HP,
        ResourceUpdateEventBundle<ShieldUpdateRequestEvent, ShieldUpdateResponseEvent> Shield,
        ResourceUpdateEventBundle<DisableUpdateRequestEvent, DisableUpdateResponseEvent> Disable
    );

    public record InteractionEventBundle(
        ResourceUpdateEventBundle ResourceUpdate,
        EventBus<LockOnRequestEvent> LockOn,
        EventBus<SkillEndEvent> SkillEnd
    );
}