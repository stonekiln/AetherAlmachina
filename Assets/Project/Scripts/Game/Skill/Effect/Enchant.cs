using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Contracts;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class EnchantEffect : SkillEffect<EnchantParameter>
    {
        protected override void ApplyTyped(ICombatInteraction user, ICombatInteraction target, EnchantParameter parameter)
        {
            parameter.Modifier.ModifierType.Enchant(user, target, parameter.Modifier, parameter.Value).Signed(parameter.Contract);
        }
    }

    [Serializable]
    public class EnchantParameter : EffectParameter
    {
        [field: SerializeField] public ModifierAsset Modifier { get; private set; }
        [field: SerializeReference] public EnchantContract Contract { get; private set; }
        [field: SerializeField] public float Value { get; private set; } = 1f;
    }
}