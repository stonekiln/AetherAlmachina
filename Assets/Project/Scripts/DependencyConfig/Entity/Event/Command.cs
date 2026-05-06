using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{

    public record AttackEventBundle(EventBus<AttackActivation> Activation, EventBus<DamageCalculation> Calculation, EventBus<DamageApplyEvent> Apply);
    /// <summary>
    /// 攻撃を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Target">対象</param>
    /// <param name="SkillPower">スキル威力</param>
    public record AttackActivation(IEntityInteraction Target, float SkillPower) : EventObject;
    public record DamageCalculation(int Attack, float Power) : EventObject;
    /// <summary>
    /// ダメージを受けたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Value">ダメージ量</param>
    public record DamageApplyEvent(int Value) : EventObject;
    public record HealEventBundle(EventBus<HealActivationEvent> Activation, EventBus<RecoveryCalculation> Calculation, EventBus<RecoveryApplyEvent> Apply);
    /// <summary>
    /// 回復を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Target">対象</param>
    /// <param name="SkillPower">スキル威力</param>
    public record HealActivationEvent(IEntityInteraction Target, float SkillPower) : EventObject;
    public record RecoveryCalculation(int Recovery, float Power) : EventObject;
    /// <summary>
    /// 回復されたことを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Recovery">回復力</param>
    /// <param name="Power">スキル威力</param>
    public record RecoveryApplyEvent(int Value) : EventObject;

    public record ActionEventBundle(
        AttackEventBundle Attack,
        HealEventBundle Heal
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