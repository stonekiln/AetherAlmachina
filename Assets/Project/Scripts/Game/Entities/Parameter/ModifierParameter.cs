using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    public record ModifierValues(IModifierTypeData Data, List<float> Values);

    public abstract class ModifierParameter
    {
        public Dictionary<StatusType, float> Value => CalcSum();
        public IEnumerable<Type> ModifierTypes => Modifiers.Keys;
        protected Dictionary<StatusType, float> Default { get; init; }
        protected Dictionary<Type, ModifierValues> Modifiers { get; init; }

        public ModifierParameter()
        {
            Default = new();
            Modifiers = new();
        }
        public Action AddModifier(IModifierData modifierData)
        {
            Type modifierType = modifierData.ModifierType;
            if (!Modifiers.TryGetValue(modifierType, out ModifierValues modifier))
            {
                Modifiers[modifierType] = modifier = new(modifierData, new());
            }

            modifier.Values.Add(modifierData.Value);

            Debug.Log(modifierData.Name + ":" + (modifierData.Value - 1) * 100 + modifierData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(modifierData);
        }
        public void RemoveModifier(IModifierData modifierData)
        {
            Modifiers[modifierData.ModifierType].Values.Remove(modifierData.Value);
            Debug.Log(modifierData.Name + ":" + (modifierData.Value - 1) * 100 + modifierData.DisplayUnit + "の効果が削除された。");
        }
        protected abstract Dictionary<StatusType, float> CalcSum();
    }

    public class FlatModifierParameter : ModifierParameter
    {
        const float DefaultValue = 0f;
        public FlatModifierParameter(Dictionary<StatusType, float> status)
        {
            foreach (StatusType type in status.Keys)
            {
                Default[type] = DefaultValue;
            }
        }
        protected override Dictionary<StatusType, float> CalcSum()
        {
            Dictionary<StatusType, float> result = new(Default);

            foreach (ModifierValues modifier in Modifiers.Values)
            {
                result[modifier.Data.StatusTypeKey] += modifier.Values.DefaultIfEmpty(DefaultValue).Max();
            }

            return result;
        }
    }

    public class RateModifierParameter : ModifierParameter
    {
        const float DefaultValue = 1f;
        public RateModifierParameter(Dictionary<StatusType, float> status)
        {
            foreach (StatusType type in status.Keys)
            {
                Default[type] = DefaultValue;
            }
        }

        protected override Dictionary<StatusType, float> CalcSum()
        {
            Dictionary<StatusType, float> result = new(Default);

            foreach (ModifierValues modifier in Modifiers.Values)
            {
                result[modifier.Data.StatusTypeKey] += modifier.Values.DefaultIfEmpty(DefaultValue).Max();
            }

            return result;
        }
    }
}