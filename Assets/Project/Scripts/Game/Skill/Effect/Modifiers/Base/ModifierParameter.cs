using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public class CommonModifierValue
    {
        public ModifierTypeData TypeData { get; init; }
        List<float> Values { get; init; }
        public CommonModifierValue(ModifierTypeData typeData)
        {
            TypeData = typeData;
            Values = new();
        }
        public float Add(float value)
        {
            Values.Add(value);
            return Max();
        }
        public float Remove(float value)
        {
            Values.Remove(value);
            return Max();
        }
        public float Max()
        {
            return Values.Aggregate(0f, (pre, cur) => Mathf.Abs(cur) > Mathf.Abs(pre) ? cur : pre);
        }
    }
    /// <summary>
    /// Modifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public abstract class ModifierParameter
    {
        /// <summary>
        /// 付与されているModifierの種類
        /// </summary>
        protected Dictionary<StatusType, float> ModifierSum { get; init; }
        //TypeをKeyとするのはType(Modifier)によってバフ効果の重複を判別するため
        //StatusTypeとほとんど同義であるが、重複しない特殊なバフや複数のステータスが変化するModifierなどが考えられるため
        Dictionary<Type, Dictionary<Type, CommonModifierValue>> Modifiers { get; init; }

        public ModifierParameter(Dictionary<StatusType, float> status)
        {
            Modifiers = new();
            ModifierSum = new();
            foreach (StatusType key in status.Keys)
            {
                ModifierSum[key] = 0f;
            }
        }

        /// <summary>
        /// Modifierを追加する
        /// </summary>
        /// <param name="modifierData">追加するModifier</param>
        /// <returns>自身のmodifierを解除するための関数</returns>
        public Action AddModifier(CommonModifierData modifierData)
        {
            Type modifierType = modifierData.ModifierType;
            Type polarity = modifierData.PolarityType;
            if (!Modifiers.ContainsKey(modifierType))
            {
                Modifiers[modifierType] = new();
            }
            if (!Modifiers[modifierType].ContainsKey(polarity))
            {
                Modifiers[modifierType][polarity] = new(modifierData.TypeData);
            }
            float preMax = Modifiers[modifierType][polarity].Max();
            float delta = Modifiers[modifierType][polarity].Add(modifierData.Value) - preMax;
            ModifierSum[modifierData.StatusTypeKey] += delta;

            Debug.Log(modifierData.TypeData.Name + ":" + modifierData.Value + modifierData.TypeData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(modifierData);
        }
        /// <summary>
        /// Modifierのを削除する
        /// </summary>
        /// <param name="modifierData">削除するModifier</param>
        void RemoveModifier(CommonModifierData modifierData)
        {
            Type modifierType = modifierData.ModifierType;
            Type polarity = modifierData.PolarityType;

            float preMax = Modifiers[modifierType][polarity].Max();
            float delta = Modifiers[modifierType][polarity].Remove(modifierData.Value) - preMax;
            ModifierSum[modifierData.StatusTypeKey] += delta;

            Debug.Log(modifierData.TypeData.Name + ":" + modifierData.Value + modifierData.TypeData.DisplayUnit + "の効果が解除された。");
        }
        /// <summary>
        /// 補正値の計算方法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumValue"></param>
        /// <returns></returns>
        protected abstract float CalcValue(float value);
        public float GetValue(StatusType statusTypeKey)
        {
            return CalcValue(ModifierSum[statusTypeKey]);
        }
    }
    /// <summary>
    /// 定数変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public class FlatModifierParameter : ModifierParameter
    {
        public FlatModifierParameter(Dictionary<StatusType, float> status) : base(status) { }

        protected override float CalcValue(float value)
        {
            return value;
        }
    }
    /// <summary>
    /// 割合変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public class RateModifierParameter : ModifierParameter
    {
        public RateModifierParameter(Dictionary<StatusType, float> status) : base(status) { }

        protected override float CalcValue(float value)
        {
            return 1f + (value / 100f);
        }
    }
}