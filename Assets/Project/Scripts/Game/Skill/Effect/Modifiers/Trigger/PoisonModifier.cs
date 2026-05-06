using System;
using AetherAlmachina.Entities;
using R3;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class PoisonModifier : TriggerModifier
    {
        public override string DisplayUnit => "";

        protected override TriggerModifierData MakeTriggerData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                Value = data.Value,
                CallBack = (func) => Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ => target.Action.Attack.Apply.OnNext(new(func())))
            };
        }
    }
}