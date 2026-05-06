using System;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class DisableDamageModifier : TriggerModifier
    {
        public override string DisplayUnit => "回";

        protected override TriggerModifierData MakeTriggerData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            throw new System.NotImplementedException();
        }
    }
}