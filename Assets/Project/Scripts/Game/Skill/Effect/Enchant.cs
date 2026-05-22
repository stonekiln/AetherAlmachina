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
    /// Modifierの種類と効果量をインスペクター上で指定できるようにする
    /// </summary>
    [Serializable]
    public class ModifierEnchantData
    {
        [field: SerializeField] ModifierAsset Type { get; set; }
        [field: SerializeField] public float Value { get; set; }

        /// <summary>
        /// Modifierを付与する対象を決める
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象候補</param>
        /// <returns>解除を行うための情報</returns>
        public DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target)
        {
            Action dispel = Type.ModifierType.MakeDispel(user, target, new(Type, Value));
            return new(user, target, dispel);
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
        [field: SerializeField] public ModifierEnchantData Modifier { get; private set; }
        /// <summary>
        /// エフェクト解除のタイミング
        /// </summary>
        [field: SerializeReference] public EnchantContract Contract { get; private set; }
    }
}