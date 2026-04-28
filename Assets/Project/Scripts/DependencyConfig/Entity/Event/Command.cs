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
    /// <summary>
    /// ダメージを受けたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Attack">攻撃力</param>
    /// <param name="Power">スキル威力</param>
    public record DamageEvent(int Attack, float Power) : EventObject;
    /// <summary>
    /// 回復を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Target">対象</param>
    /// <param name="SkillPower">スキル威力</param>
    public record HealingEvent(IEntityInteraction Target, float SkillPower) : EventObject;
    /// <summary>
    /// 回復されたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Recovery">回復力</param>
    /// <param name="Power">スキル威力</param>
    public record OnHealedEvent(int Recovery, float Power) : EventObject;

    public record ActionEventBundle(
        EventBus<AttackEvent> Attack,
        EventBus<DamageEvent> Damage,
        EventBus<HealingEvent> Healing,
        EventBus<OnHealedEvent> OnHealed
    );

    /// <summary>
    /// スキルが終了したことを宣言するイベントメッセージ
    /// </summary>
    public record SkillEndEvent : EventObject;
    /// <summary>
    /// MPが変化したことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Delta">変化量</param>
    public record MPUpdateEvent(int Delta) : EventObject;

    public record ProcessEventBundle(
        EventBus<SkillEndEvent> SkillEnd,
        EventBus<MPUpdateEvent> MPUpdate
    );
}