using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    public abstract class ModifierParameterBase
    {
        public Dictionary<StatusType, float> Value => CalcSum();
        public IEnumerable<Type> ModifierTypes => Modifiers.Keys;
        protected Dictionary<StatusType, float> Default { get; init; }
        protected Dictionary<Type, Dictionary<StatusType, List<float>>> Modifiers { get; init; }
        public ModifierParameterBase()
        {
            Default = new();
            Modifiers = new();
        }

        public void AddModifier(Type modifierType, StatusType statusType, float value)
        {
            if (!Modifiers.TryGetValue(modifierType, out Dictionary<StatusType, List<float>> modifier))
            {
                Modifiers[modifierType] = modifier = new();
            }
            if (!modifier.TryGetValue(statusType, out List<float> list))
            {
                modifier[statusType] = list = new();
            }

            list.Add(value);
        }

        public void RemoveModifier(Type modifierType, StatusType statusType, float value)
        {
            Modifiers[modifierType][statusType].Remove(value);
            Debug.Log(modifierType + ":" + value + "の効果が削除された。");
        }

        protected abstract Dictionary<StatusType, float> CalcSum();
    }

    public class FlatModifierParameter : ModifierParameterBase
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

            foreach (Dictionary<StatusType, List<float>> modifier in Modifiers.Values)
            {
                foreach (StatusType type in modifier.Keys)
                {
                    result[type] += modifier[type].Max();
                }
            }

            return result;
        }
    }

    public class PercentModifierParameter : ModifierParameterBase
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

            foreach (Dictionary<StatusType, List<float>> modifier in Modifiers.Values)
            {
                foreach (StatusType type in modifier.Keys)
                {
                    result[type] *= modifier[type].Max();
                }
            }

            return result;
        }
    }
}