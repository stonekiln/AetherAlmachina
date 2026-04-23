using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record SkillEndEvent : EventObject;
    public record AttackEvent(Entity Target, float SkillPower) : EventObject;
    public record DamageEvent(int Attack, float Power) : EventObject;
    public record HealingEvent(Entity Target, float SkillPower) : EventObject;
    public record OnHealedEvent(int Attack, float Power) : EventObject;

    public record CommandEventBundle(
        EventBus<SkillEndEvent> SkillEnd,
        EventBus<AttackEvent> Attack,
        EventBus<DamageEvent> Damage,
        EventBus<HealingEvent> Healing,
        EventBus<OnHealedEvent> OnHealed);
}