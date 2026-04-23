using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record TargetingEventBundle(LockOnEvent LockOn, EventBus<HitEvent> Hit);
    public class LockOnEvent : EventChannel<LockOnRequestEvent, LockOnResponseEvent> { };
    public record LockOnRequestEvent(Func<IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>> Selector) : EventObject;
    public record LockOnResponseEvent(IEnumerable<IEntityInteraction> Targets) : EventObject;
    public record HitEvent(Action<Entity> Apply) : EventObject;
}