using System;
using System.Collections.Generic;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
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
        Dictionary<Type, Dictionary<Type, ModifierValues>> Modifiers { get; init; }

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
            Modifiers[modifierType][polarity].Add(modifierData.SignedValue);
            if (MathF.Abs(preMax) < Mathf.Abs(modifierData.SignedValue))
            {
                ModifierSum[modifierData.StatusTypeKey] += modifierData.SignedValue - preMax;
            }

            Debug.Log(modifierData.TypeData.Name + ":" + modifierData.SignedValue + modifierData.TypeData.DisplayUnit + " の効果が付与された。");
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

            Modifiers[modifierType][polarity].Remove(modifierData.SignedValue);
            float curMax = Modifiers[modifierType][polarity].Max();
            if (Mathf.Abs(curMax) < Mathf.Abs(modifierData.SignedValue))
            {
                ModifierSum[modifierData.StatusTypeKey] += curMax - modifierData.SignedValue;
            }

            Debug.Log(modifierData.TypeData.Name + ":" + modifierData.SignedValue + modifierData.TypeData.DisplayUnit + "の効果が解除された。");
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