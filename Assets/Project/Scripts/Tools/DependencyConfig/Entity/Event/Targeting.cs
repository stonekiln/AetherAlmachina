using System;
using System.Collections.Generic;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record AttackEventBundle(TargetingEvent Targeting, EventBus<HitEvent> Hit);
    public class TargetingEvent : EventChannel<TargetingRequestEvent, TargetingResponseEvent> { };
    public record TargetingRequestEvent(Func<IEnumerable<ICombatInteraction>, IEnumerable<ICombatInteraction>, IEnumerable<ICombatInteraction>> TargetSetter) : EventObject;
    public record TargetingResponseEvent(IEnumerable<ICombatInteraction> Targets) : EventObject;
    public record HitEvent(Action<Entity> Apply) : EventObject;

    public record SkillActiveEvent(SkillData Data) : EventObject;
}