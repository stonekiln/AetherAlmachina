using System;
using System.Collections.Generic;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public abstract class ModifierParameter : ModifierStock<CommonModifierData>
    {
        /// <summary>
        /// 付与されているModifierの種類
        /// </summary>
        protected Dictionary<StatusType, float> ModifierSum { get; init; }

        public ModifierParameter(Dictionary<StatusType, float> status)
        {
            Modifiers = new();
            ModifierSum = new();
            foreach (StatusType key in status.Keys)
            {
                ModifierSum[key] = 0f;
            }
        }

        public override Action AddModifier(CommonModifierData data)
        {
            Type modifierType = data.ModifierType;
            Type polarity = data.PolarityType;

            CreateKey(modifierType, polarity, data.TypeData);

            float curMax = Modifiers[modifierType][polarity].Max();
            if (MathF.Abs(curMax) < Mathf.Abs(data.ModifyValue))
            {
                ModifierSum[data.StatusTypeKey] += data.ModifyValue - curMax;
            }

            Modifiers[modifierType][polarity].Add(data.ModifyValue);

            Debug.Log(data.TypeData.Name + ":" + data.ModifyValue + data.TypeData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(data);
        }
        protected override void RemoveModifier(CommonModifierData data)
        {
            Type modifierType = data.ModifierType;
            Type polarity = data.PolarityType;

            Modifiers[modifierType][polarity].Remove(data.ModifyValue);

            float curMax = Modifiers[modifierType][polarity].Max();
            if (Mathf.Abs(curMax) < Mathf.Abs(data.ModifyValue))
            {
                ModifierSum[data.StatusTypeKey] += curMax - data.ModifyValue;
            }

            Debug.Log(data.TypeData.Name + ":" + data.ModifyValue + data.TypeData.DisplayUnit + "の効果が解除された。");
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