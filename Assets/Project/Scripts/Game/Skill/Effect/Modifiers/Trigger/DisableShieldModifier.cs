using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class DisableShieldModifier : TriggerModifier
    {
        public override string DisplayUnit => "(回)";

        protected override TriggerModifierData MakeModifierData(IEntityInteraction user, IEntityInteraction target, ModifierData data)
        {
            return new(data);
        }
    }
}