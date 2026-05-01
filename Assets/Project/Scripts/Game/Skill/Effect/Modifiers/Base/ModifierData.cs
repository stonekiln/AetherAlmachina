using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public record ModifierTypeData(string Name, Sprite Icon, string DisplayUnit);
    public class TriggerModifierData
    {
        public ModifierTypeData TypeData { get; init; }
        public Type ModifierType { get; init; }
        public Type PolarityType { get; init; }
        public ModifierPolarity Polarity { get; init; }
        float value;
        public float Value => Polarity.Get(value);
        public TriggerModifierData(ModifierEnchantData modifierData)
        {
            ModifierType = modifierData.Type.ModifierType.GetType();
            Polarity = modifierData.Type.Polarity;
            PolarityType = Polarity.GetType();
            value = modifierData.Value;
            TypeData = new(modifierData.Type.Name, modifierData.Type.Icon, modifierData.Type.ModifierType.DisplayUnit);
        }
    }
    public class CommonModifierData : TriggerModifierData
    {
        public StatusType StatusTypeKey { get; init; }
        public CommonModifierData(ModifierEnchantData modifierData) : base(modifierData)
        {
            CommonModifier commonModifier = modifierData.Type.ModifierType as CommonModifier;
            StatusTypeKey = commonModifier.StatusTypeKey;
        }
    }

    /// <summary>
    /// Modifierの情報
    /// </summary>
    [Serializable]
    public class ModifierEnchantData
    {
        [field: SerializeField] public ModifierAsset Type { get; private set; }
        [field: SerializeField] public float Value { get; private set; }

        /// <summary>
        /// Modifierを付与する対象を決める
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象候補</param>
        /// <returns>解除を行うための情報</returns>
        public DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target)
        {
            return Type.ModifierType.Enchant(user, target, this);
        }
    }
}