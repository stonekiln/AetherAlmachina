using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの効果の固有情報
    /// </summary>
    /// <param name="Name">効果名</param>
    /// <param name="Icon">効果アイコン</param>
    /// <param name="DisplayUnit">効果量の単位</param>
    public record ModifierTypeData(string Name, Sprite Icon, string DisplayUnit);
    /// <summary>
    /// Modifierの固有情報と効果値を1対多対応でまとめて扱いやすくするためのクラス
    /// </summary>
    public class ModifierValues
    {
        /// <summary>
        /// Modifierの情報
        /// </summary>
        public ModifierTypeData TypeData { get; init; }
        List<float> Values { get; init; }
        public ModifierValues(ModifierTypeData typeData)
        {
            TypeData = typeData;
            Values = new();
        }
        /// <summary>
        /// 効果値一覧に追加する
        /// </summary>
        /// <param name="value">追加する効果量</param>
        public void Add(float value)
        {
            Values.Add(value);
        }
        /// <summary>
        /// 効果値一覧から削除する
        /// </summary>
        /// <param name="value">削除する効果量</param>
        public void Remove(float value)
        {
            Values.Remove(value);
        }
        /// <summary>
        /// 効果値の一覧から最大値を取得し、それを実効値とする
        /// </summary>
        /// <returns>実効値</returns>
        public float Max()
        {
            //マイナス効果の値も符号付きで記録するため、絶対値で計算する
            return Values.Aggregate(0f, (pre, cur) => Mathf.Abs(cur) > Mathf.Abs(pre) ? cur : pre);
        }
    }
    /// <summary>
    /// Modifierの固有情報と効果値を1対1対応でまとめて扱いやすくするためのクラス
    /// </summary>
    public class ModifierRawData
    {
        /// <summary>
        /// Modifierの固有情報
        /// </summary>
        public ModifierTypeData TypeData { get; init; }
        /// <summary>
        /// Modifierの型情報(型名を基準にModifier同士が重複する)
        /// </summary>
        public Type ModifierType { get; init; }
        /// <summary>
        /// その効果がプラス効果かマイナス効果か
        /// </summary>
        public ModifierPolarity Polarity { get; init; }
        /// <summary>
        /// インスペクターに表示されている効果量
        /// </summary>
        public float Value { get; init; }
        /// <summary>
        /// Valueの数値から主にPolarityより正負の符号をつけた値。もしくは、ステータス参照系など効果量がバラバラな場合の値。(実際に記録される数値はこちらの値)
        /// </summary>
        public float ModifyValue { get; init; }

        public ModifierRawData(ModifierAsset asset, float value)
        {
            ModifierType = asset.Definition.GetType();
            Polarity = asset.Polarity;
            Value = value;
            ModifyValue = Polarity.ApplySign(value);
            TypeData = new(asset.Name, asset.Icon, asset.Definition.DisplayUnit);
        }
        public ModifierRawData(ModifierRawData data)
        {
            ModifierType = data.ModifierType;
            Polarity = data.Polarity;
            Value = data.Value;
            ModifyValue = data.ModifyValue;
            TypeData = data.TypeData;
        }
    }
    /// <summary>
    /// Modifierの固有情報と効果値を1対1対応でまとめて扱いやすくするためのクラスに、Modifierが追加されるときの具体的な処理を定義するためのクラス
    /// </summary>
    public class ModifierData : ModifierRawData
    {
        /// <summary>
        /// 効果量が追加された場合の処理(引数floatは追加前の効果量最大値)
        /// </summary>
        public Action<float> AddCallBack { get; init; }
        /// <summary>
        /// 効果量が削除された場合の処理(引数floatは削除後の効果量最大値)
        /// </summary>
        public Action<float> RemoveCallBack { get; init; }

        public ModifierData(ModifierRawData rawData, Action<float> addCallBack, Action<float> removeCallBack) : base(rawData)
        {
            AddCallBack = addCallBack;
            RemoveCallBack = removeCallBack;
        }
    }
}