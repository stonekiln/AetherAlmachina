using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// 攻撃を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Target">対象</param>
    /// <param name="SkillPower">スキル威力</param>
    public record AttackEvent(IEntityInteraction Target, float SkillPower) : EventObject;
    public record DamageEvent(int Attack, float Power) : EventObject;
    /// <summary>
    /// 回復を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Target">対象</param>
    /// <param name="SkillPower">スキル威力</param>
    public record HealEvent(IEntityInteraction Target, float SkillPower) : EventObject;
    public record RecoveryEvent(int Recovery, float Power) : EventObject;

    public record ActionEventBundle(
        EventBus<AttackEvent> Attack,
        EventBus<DamageEvent> Damage,
        EventBus<HealEvent> Heal,
        EventBus<RecoveryEvent> Recovery
    );

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

    public record ResourceUpdateEventBundle(
        ResourceUpdateEventBundle<CostUpdateRequestEvent, CostUpdateResponseEvent> Cost,
        ResourceUpdateEventBundle<HPUpdateRequestEvent, HPUpdateResponseEvent> HP,
        ResourceUpdateEventBundle<ShieldUpdateRequestEvent, ShieldUpdateResponseEvent> Shield,
        ResourceUpdateEventBundle<DisableUpdateRequestEvent, DisableUpdateResponseEvent> Disable
    );

    public record ProcessEventBundle(
        EventBus<SkillEndEvent> SkillEnd,
        ResourceUpdateEventBundle ResourceUpdate
    );
}