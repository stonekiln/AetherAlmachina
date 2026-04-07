using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    public abstract class ModifierParameter
    {
        public record ModifierData(ModifierAsset Data, Dictionary<StatusType, List<float>> ValueData);
        public Dictionary<StatusType, float> Value => CalcSum();
        public IEnumerable<Type> ModifierTypes => Modifiers.Keys;
        protected Dictionary<StatusType, float> Default { get; init; }
        protected Dictionary<Type, ModifierData> Modifiers { get; init; }

        public ModifierParameter()
        {
            Default = new();
            Modifiers = new();
        }
        public Action AddModifier(ModifierAsset modifierAsset, StatusType statusType, float value)
        {
            Type modifierType = modifierAsset.ModifierType.GetType();
            if (!Modifiers.TryGetValue(modifierType, out ModifierData modifier))
            {
                Modifiers[modifierType] = modifier = new(modifierAsset, new());
            }
            if (!modifier.ValueData.TryGetValue(statusType, out List<float> list))
            {
                modifier.ValueData[statusType] = list = new();
            }
            list.Add(value);

            Debug.Log(modifierAsset.Name + ":" + value + " の効果が付与された。");
            return () => RemoveModifier(modifierAsset, statusType, value);
        }
        public void RemoveModifier(ModifierAsset modifierAsset, StatusType statusType, float value)
        {
            Modifiers[modifierAsset.ModifierType.GetType()].ValueData[statusType].Remove(value);
            Debug.Log(modifierAsset.Name + ":" + value + "の効果が削除された。");
        }
        protected abstract Dictionary<StatusType, float> CalcSum();
    }

    public class FlatModifierParameter : ModifierParameter
    {
        public FlatModifierParameter(Dictionary<StatusType, float> status)
        {
            foreach (StatusType type in status.Keys)
            {
                Default[type] = 0f;
            }
        }
        protected override Dictionary<StatusType, float> CalcSum()
        {
            Dictionary<StatusType, float> result = new(Default);

            foreach (ModifierData modifier in Modifiers.Values)
            {
                foreach (StatusType type in modifier.ValueData.Keys)
                {
                    result[type] += modifier.ValueData[type].Max();
                }
            }

            return result;
        }
    }

    public class PercentModifierParameter : ModifierParameter
    {
        public PercentModifierParameter(Dictionary<StatusType, float> status)
        {
            foreach (StatusType type in status.Keys)
            {
                Default[type] = 1f;
            }
        }

        protected override Dictionary<StatusType, float> CalcSum()
        {
            Dictionary<StatusType, float> result = new(Default);

            foreach (ModifierData modifier in Modifiers.Values)
            {
                foreach (StatusType type in modifier.ValueData.Keys)
                {
                    result[type] *= modifier.ValueData[type].Max();
                }
            }

            return result;
        }
    }
}