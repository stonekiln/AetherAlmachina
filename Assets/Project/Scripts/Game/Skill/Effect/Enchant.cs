using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class EnchantEffect : SkillEffect<EnchantParameter>
    {
        protected override void ApplyTyped(ICombatInteraction user, ICombatInteraction target, EnchantParameter parameter)
        {
            parameter.Type.Modifier.Enchant(target, parameter.Value, parameter.During);
        }
    }

    [Serializable]
    public class EnchantParameter : EffectParameter
    {
        [field: SerializeField] public ModifierType Type { get; private set; }
        [field: SerializeField] public float Value { get; private set; } = 1f;
        [field: SerializeField] public float During { get; private set; } = 1f;
    }
}