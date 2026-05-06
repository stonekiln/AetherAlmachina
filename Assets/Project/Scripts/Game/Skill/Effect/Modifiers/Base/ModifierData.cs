using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public record ModifierTypeData(string Name, Sprite Icon, string DisplayUnit);
    public class ModifierRawData
    {
        public ModifierTypeData TypeData { get; init; }
        public Type ModifierType { get; init; }
        public Type PolarityType { get; init; }
        public ModifierPolarity Polarity { get; init; }
        public float Value { get; init; }
        public float SignedValue => Polarity.GetValue(Value);
        public ModifierRawData(ModifierAsset asset, float value)
        {
            ModifierType = asset.ModifierType.GetType();
            Polarity = asset.Polarity;
            PolarityType = Polarity.GetType();
            Value = value;
            TypeData = new(asset.Name, asset.Icon, asset.ModifierType.DisplayUnit);
        }
        public ModifierRawData(ModifierRawData data)
        {
            ModifierType = data.ModifierType;
            Polarity = data.Polarity;
            PolarityType = data.PolarityType;
            Value = data.Value;
            TypeData = data.TypeData;
        }
    }
    public class CommonModifierData : ModifierRawData
    {
        public StatusType StatusTypeKey { get; init; }
        public CommonModifierData(ModifierRawData data) : base(data) { }
    }
    public class TriggerModifierData : ModifierRawData
    {
        public Func<Func<int>, IDisposable> CallBack { get; init; }
        public TriggerModifierData(ModifierRawData data) : base(data) { }
    }

    /// <summary>
    /// Modifierの情報
    /// </summary>
    [Serializable]
    public class ModifierEnchantData
    {
        [SerializeField] ModifierAsset type;
        [SerializeField] public float value;

        /// <summary>
        /// Modifierを付与する対象を決める
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象候補</param>
        /// <returns>解除を行うための情報</returns>
        public DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target)
        {
            Action dispel = type.ModifierType.MakeDispel(user, target, new(type, value));
            return new(user, target, dispel);
        }
    }
}