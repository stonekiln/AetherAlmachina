using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record TargetingEventBundle(LockOnEvent LockOn, EventBus<HitEvent> Hit);
    public class LockOnEvent : EventChannel<LockOnRequestEvent, LockOnResponseEvent> { };
    public record LockOnRequestEvent(Func<IEnumerable<ICombatInteraction>, IEnumerable<ICombatInteraction>, IEnumerable<ICombatInteraction>> Selector) : EventObject;
    public record LockOnResponseEvent(IEnumerable<ICombatInteraction> Targets) : EventObject;
    public record HitEvent(Action<Entity> Apply) : EventObject;

    public record SkillActiveEvent(SkillData Data) : EventObject;
}