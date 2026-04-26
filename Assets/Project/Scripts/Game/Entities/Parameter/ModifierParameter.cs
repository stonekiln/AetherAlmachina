using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// Modifierの数値情報を渡す
    /// </summary>
    public class ModifierValueData
    {
        /// <summary>
        /// 名称やアイコンなどの基本情報
        /// </summary>
        public IModifierData Data { get; init; }
        /// <summary>
        /// Modifierの実効値
        /// </summary>
        public float Value => CalcRMS();
        /// <summary>
        /// Modifierの効果量の一覧
        /// </summary>
        List<float> Values { get; init; }

        public ModifierValueData(IModifierData data, float defaultValue)
        {
            Data = data;
            Values = new() { defaultValue };
        }

        /// <summary>
        /// 同じ種類のModifierの実効値を求める
        /// </summary>
        /// <returns>Modifier実効値</returns>
        float CalcRMS()
        {
            float max = 0;
            float min = 0;
            foreach (float value in Values)
            {
                if (value > max) max = value;
                if (value < min) min = value;
            }

            return max + min;
        }
        /// <summary>
        /// 効果量リストに指定した数値を追加する
        /// </summary>
        /// <param name="value">追加する数値</param>
        public void Add(float value)
        {
            Values.Add(value);
        }
        /// <summary>
        /// 効果量リストから指定した数値を削除する
        /// </summary>
        /// <param name="value">削除する数値</param>
        public void Remove(float value)
        {
            Values.Remove(value);
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
        public IEnumerable<Type> ModifierTypes => Modifiers.Keys;
        //TypeをKeyとするのはType(Modifier)によってバフ効果の重複を判別するため
        //StatusTypeとほとんど同義であるが、重複しない特殊なバフや複数のステータスが変化するModifierなどが考えられるため
        protected Dictionary<Type, ModifierValueData> Modifiers { get; init; }
        /// <summary>
        /// 初期値が定数と割合で異なるためそれぞれ指定する
        /// </summary>
        protected abstract float DefaultValue { get; }

        public ModifierParameter()
        {
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
                Modifiers[modifierType] = new(modifierData, DefaultValue);
            }
            Modifiers[modifierType].Add(modifierData.Value);

            Debug.Log(modifierData.Name + ":" + modifierData.Value + modifierData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(modifierData);
        }
        /// <summary>
        /// Modifierのを削除する
        /// </summary>
        /// <param name="modifierData">削除するModifier</param>
        void RemoveModifier(IModifierData modifierData)
        {
            Modifiers[modifierData.ModifierType].Remove(modifierData.Value);
            Debug.Log(modifierData.Name + ":" + modifierData.Value + modifierData.DisplayUnit + "の効果が解除された。");
        }
        /// <summary>
        /// 補正値の計算方法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumValue"></param>
        /// <returns></returns>
        protected abstract float CalcValue(ModifierValueData data, float sumValue);
        /// <summary>
        /// Modifierによる補正値を取得する
        /// </summary>
        /// <param name="key">取得する能力値</param>
        /// <returns>取得した補正値</returns>
        public float GetValue(StatusType key)
        {
            return Modifiers.Values.Where(modifier => modifier.Data.StatusTypeKey == key).Aggregate(DefaultValue, (pre, cur) => CalcValue(cur, pre));
        }
    }
    /// <summary>
    /// 定数変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public class FlatModifierParameter : ModifierParameter
    {
        protected override float DefaultValue => 0f;

        protected override float CalcValue(ModifierValueData data, float sumValue)
        {
            return sumValue + data.Value;
        }
    }
    /// <summary>
    /// 割合変化のModifierによるパラメータの補正値を管理するクラス
    /// </summary>
    public class RateModifierParameter : ModifierParameter
    {
        protected override float DefaultValue => 1f;

        protected override float CalcValue(ModifierValueData data, float sumValue)
        {
            return sumValue + (data.Value / 100f);
        }
    }
}