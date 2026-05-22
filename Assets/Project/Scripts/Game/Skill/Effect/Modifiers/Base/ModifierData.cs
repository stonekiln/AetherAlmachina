using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public record ModifierTypeData(string Name, Sprite Icon, string DisplayUnit);
    public class ModifierValues
    {
        public ModifierTypeData TypeData { get; init; }
        List<float> Values { get; init; }
        public ModifierValues(ModifierTypeData typeData)
        {
            TypeData = typeData;
            Values = new();
        }
        public void Add(float value)
        {
            Values.Add(value);
        }
        public void Remove(float value)
        {
            Values.Remove(value);
        }
        public float Max()
        {
            return Values.Aggregate(0f, (pre, cur) => Mathf.Abs(cur) > Mathf.Abs(pre) ? cur : pre);
        }
    }
    public class ModifierRawData
    {
        public ModifierTypeData TypeData { get; init; }
        public Type ModifierType { get; init; }
        public Type PolarityType { get; init; }
        public ModifierPolarity Polarity { get; init; }
        public float Value { get; init; }
        public float ModifyValue { get; init; }
        public ModifierRawData(ModifierAsset asset, float value)
        {
            ModifierType = asset.ModifierType.GetType();
            Polarity = asset.Polarity;
            PolarityType = Polarity.GetType();
            Value = value;
            ModifyValue = Polarity.ApplySign(value);
            TypeData = new(asset.Name, asset.Icon, asset.ModifierType.DisplayUnit);
        }
        public ModifierRawData(ModifierRawData data)
        {
            ModifierType = data.ModifierType;
            Polarity = data.Polarity;
            PolarityType = data.PolarityType;
            Value = data.Value;
            ModifyValue = ModifyValue;
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
        public Action AddCallBack { get; init; }
        public Action RemoveCallBack { get; init; }
        public TriggerModifierData(ModifierRawData data) : base(data) { }
    }
}