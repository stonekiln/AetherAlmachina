using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
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

    public class TriggerModifiers
    {
        class TriggerObservable
        {
            public ModifierValues Values { get; init; }
            public IDisposable disposable;
            public TriggerObservable(ModifierValues values)
            {
                Values = values;
            }
        }
        //TypeをKeyとするのはType(Modifier)によってバフ効果の重複を判別するため
        Dictionary<Type, Dictionary<Type, TriggerObservable>> Modifiers { get; init; }

        public TriggerModifiers()
        {
            Modifiers = new();
        }

        /// <summary>
        /// Modifierを追加する
        /// </summary>
        /// <param name="modifierData">追加するModifier</param>
        /// <returns>自身のmodifierを解除するための関数</returns>
        public Action AddModifier(TriggerModifierData modifierData)
        {
            Type modifierType = modifierData.ModifierType;
            Type polarity = modifierData.PolarityType;
            if (!Modifiers.ContainsKey(modifierType))
            {
                Modifiers[modifierType] = new();
            }
            if (!Modifiers[modifierType].ContainsKey(polarity))
            {
                Modifiers[modifierType][polarity] = new(new(modifierData.TypeData));
            }
            bool isSubscribe = true;
            if (Modifiers[modifierType][polarity].Values.Max() == 0)
            {
                isSubscribe = false;
            }
            Modifiers[modifierType][polarity].Values.Add(modifierData.SignedValue);
            if (!isSubscribe)
            {
                Modifiers[modifierType][polarity].disposable = modifierData.CallBack(() => Mathf.FloorToInt(Modifiers[modifierType][polarity].Values.Max()));
            }

            Debug.Log(modifierData.TypeData.Name + ":" + modifierData.SignedValue + modifierData.TypeData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(modifierData);
        }
        /// <summary>
        /// Modifierのを削除する
        /// </summary>
        /// <param name="modifierData">削除するModifier</param>
        void RemoveModifier(TriggerModifierData modifierData)
        {
            Type modifierType = modifierData.ModifierType;
            Type polarity = modifierData.PolarityType;
            Modifiers[modifierData.ModifierType][modifierData.PolarityType].Values.Remove(modifierData.SignedValue);
            if (Modifiers[modifierType][polarity].Values.Max() == 0)
            {
                Modifiers[modifierData.ModifierType][modifierData.PolarityType].disposable.Dispose();
            }

            Debug.Log(modifierData.TypeData.Name + ":" + modifierData.SignedValue + modifierData.TypeData.DisplayUnit + "の効果が解除された。");
        }
        public float GetValue(TriggerModifierData modifierData)
        {
            return Modifiers[modifierData.ModifierType][modifierData.PolarityType].Values.Max();
        }
    }
}