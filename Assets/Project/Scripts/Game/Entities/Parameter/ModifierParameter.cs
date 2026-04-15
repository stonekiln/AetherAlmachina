using System;
using System.Collections.Generic;
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
            if (!Modifiers.ContainsKey(modifierType))
            {
                Modifiers[modifierType] = new(modifierData, new() { 0f });
            }
            Modifiers[modifierType].Values.Add(modifierData.Value);

            Debug.Log(modifierData.Name + ":" + modifierData.Value + modifierData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(modifierData);
        }
        public void RemoveModifier(IModifierData modifierData)
        {
            Modifiers[modifierData.ModifierType].Values.Remove(modifierData.Value);
            Debug.Log(modifierData.Name + ":" + modifierData.Value + modifierData.DisplayUnit + "の効果が解除された。");
        }
        protected float CalcRMS(List<float> list)
        {
            float max = float.NegativeInfinity;
            float min = float.PositiveInfinity;
            foreach (float value in list)
            {
                if (value > max) max = value;
                if (value < min) min = value;
            }

            return max + min;
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

            foreach (ModifierValues modifier in Modifiers.Values)
            {
                result[modifier.Data.StatusTypeKey] += CalcRMS(modifier.Values);
            }

            return result;
        }
    }

    public class RateModifierParameter : ModifierParameter
    {
        public RateModifierParameter(Dictionary<StatusType, float> status)
        {
            foreach (StatusType type in status.Keys)
            {
                Default[type] = 1f;
            }
        }

        protected override Dictionary<StatusType, float> CalcSum()
        {
            Dictionary<StatusType, float> result = new(Default);

            foreach (ModifierValues modifier in Modifiers.Values)
            {
                result[modifier.Data.StatusTypeKey] += CalcRMS(modifier.Values) / 100f;
            }

            return result;
        }
    }
}