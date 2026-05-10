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
    /// <summary>
    /// MPが変化したことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Delta">変化量</param>
    public record CostUpdateEvent(int Delta) : EventObject;
    public record HPUpdateEvent(int Delta) : EventObject;
    public record ShieldUpdateEvent(int Delta) : EventObject;
    public record DisableUpdateEvent(int Delta) : EventObject;

    public record ResourceUpdateEventBundle(
        EventBus<HPUpdateEvent> HP,
        EventBus<ShieldUpdateEvent> Shield,
        EventBus<DisableUpdateEvent> Disable
    );
    public record ProcessEventBundle(
        EventBus<SkillEndEvent> SkillEnd,
        ResourceUpdateEventBundle ResourceUpdate,
        EventBus<CostUpdateEvent> CostUpdate
    );
}