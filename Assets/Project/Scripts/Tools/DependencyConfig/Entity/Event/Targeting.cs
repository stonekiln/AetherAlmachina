using System;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record AttackEventBundle(EventBus<TargetingEvent> Targeting, EventBus<HitEvent> Hit);
    public record TargetingEvent(SkillData Data) : EventObject;
    public record HitEvent(Action<Entity> Activate) : EventObject;

    public record SkillActiveEvent(SkillData Data) : EventObject;
}