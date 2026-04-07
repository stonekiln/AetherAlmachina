using AetherAlmachina.Entities;
using AetherAlmachina.Skill;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record SkillActiveEvent(SkillData Data) : EventObject;
    public record SkillEndEvent : EventObject;
    public record AttackEvent(Entity Target, float SkillPower) : EventObject;
    public record DamageEvent(int Attack, float Power) : EventObject;
    public record HealingEvent(Entity Target, float SkillPower) : EventObject;
    public record OnHealedEvent(int Attack, float Power) : EventObject;

    public record CommandEventBundle(
        EventBus<SkillActiveEvent> SkillActive,
        EventBus<SkillEndEvent> SkillEnd,
        EventBus<AttackEvent> Attack,
        EventBus<DamageEvent> Damage,
        EventBus<HealingEvent> Healing,
        EventBus<OnHealedEvent> OnHealed);
}