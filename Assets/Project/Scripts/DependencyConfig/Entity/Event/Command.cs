using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    /// <summary>
    /// スキルが終了したことを宣言するイベントメッセージ
    /// </summary>
    public record SkillEndEvent : EventObject;
    /// <summary>
    /// 攻撃を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Target">対象</param>
    /// <param name="SkillPower">スキル威力</param>
    public record AttackEvent(Entity Target, float SkillPower) : EventObject;
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
    public record HealingEvent(Entity Target, float SkillPower) : EventObject;
    /// <summary>
    /// 回復されたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Recovery">回復力</param>
    /// <param name="Power">スキル威力</param>
    public record OnHealedEvent(int Recovery, float Power) : EventObject;

    public record CommandEventBundle(
        EventBus<SkillEndEvent> SkillEnd,
        EventBus<AttackEvent> Attack,
        EventBus<DamageEvent> Damage,
        EventBus<HealingEvent> Healing,
        EventBus<OnHealedEvent> OnHealed);
}