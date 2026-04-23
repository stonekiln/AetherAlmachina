using System;
using System.Collections.Generic;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// Modifierの情報を渡す
    /// </summary>
    /// <param name="Data">付与するModifierのデータ</param>
    /// <param name="Values">同種効果のModifierの効果量の一覧</param>
    public record ModifierValues(IModifierTypeData Data, List<float> Values);

    /// <summary>
    /// Modifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public abstract class ModifierParameter
    {
        /// <summary>
        /// Modifierによる補正値
        /// </summary>
        public Dictionary<StatusType, float> Value => CalcSum();
        /// <summary>
        /// 付与されているModifierの種類
        /// </summary>
        public IEnumerable<Type> ModifierTypes => Modifiers.Keys;
        protected Dictionary<StatusType, float> Default { get; init; }
        protected Dictionary<Type, ModifierValues> Modifiers { get; init; }

        public ModifierParameter()
        {
            Default = new();
            Modifiers = new();
        }
        /// <summary>
        /// Modifierを追加する
        /// </summary>
        /// <param name="modifierData">追加するModifier</param>
        /// <returns>自身のmodifierを解除するための関数</returns>
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
        /// <summary>
        /// Modifierのを削除する
        /// </summary>
        /// <param name="modifierData">削除するModifier</param>
        void RemoveModifier(IModifierData modifierData)
        {
            Modifiers[modifierData.ModifierType].Values.Remove(modifierData.Value);
            Debug.Log(modifierData.Name + ":" + modifierData.Value + modifierData.DisplayUnit + "の効果が解除された。");
        }
        /// <summary>
        /// 同じ種類のModifierのバフとデバフを取得し最終的な実効値を求める
        /// </summary>
        /// <param name="list">Modifierの効果量の一覧</param>
        /// <returns>Modifier実効値</returns>
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
        /// <summary>
        /// Modifierの計算方法
        /// </summary>
        /// <returns>計算後のModifierを表す辞書型</returns>
        protected abstract Dictionary<StatusType, float> CalcSum();
    }
    /// <summary>
    /// 定数変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
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
    /// <summary>
    /// 割合変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
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