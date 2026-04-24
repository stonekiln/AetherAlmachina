using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Contracts;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// 相手にModifierを付与する効果
    /// </summary>
    [Serializable]
    public class EnchantEffect : SkillEffect<EnchantParameter>
    {
        protected override void ApplyTyped(IEntityInteraction user, IEntityInteraction target, EnchantParameter parameter)
        {
            parameter.Modifier.Enchant(user, target).Signed(parameter.Contract);
        }
    }
    /// <summary>
    /// EnchantEffectに必要なパラメータ
    /// </summary>
    [Serializable]
    public class EnchantParameter : EffectParameter
    {
        /// <summary>
        /// Modifierの情報
        /// </summary>
        [field: SerializeField] public ModifierData Modifier { get; private set; }
        /// <summary>
        /// エフェクト解除のタイミング
        /// </summary>
        [field: SerializeReference] public EnchantContract Contract { get; private set; }
    }
}