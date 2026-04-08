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
            parameter.Modifier.Enchant(user, target).Signed(parameter.Contract);
        }
    }

    [Serializable]
    public class EnchantParameter : EffectParameter
    {
        [field: SerializeField] public ModifierData Modifier { get; private set; }
        [field: SerializeReference] public EnchantContract Contract { get; private set; }
    }
}