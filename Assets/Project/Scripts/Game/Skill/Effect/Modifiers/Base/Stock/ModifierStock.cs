using System;
using System.Collections.Generic;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの追加・削除を行う。また、付与済みのModifier一覧を記録する。
    /// </summary>
    public abstract class ModifierStock
    {
        //TODO:辞書型を直接参照する形にしないこと
        public Dictionary<Type, Dictionary<Type, ModifierValues>> Modifiers { get; init; }
        protected void CreateKey(Type modifier, Type polarity, ModifierTypeData data)
        {
            if (!Modifiers.ContainsKey(modifier))
            {
                Modifiers[modifier] = new();
            }
            if (!Modifiers[modifier].ContainsKey(polarity))
            {
                Modifiers[modifier][polarity] = new(data);
            }
        }
        /// <summary>
        /// Modifierを追加する
        /// </summary>
        /// <param name="data">追加するModifier</param>
        /// <returns>自身のmodifierを解除するための関数</returns>
        public Action AddModifier(ModifierData data)
        {
            Type modifierType = data.ModifierType;
            Type polarity = data.Polarity.GetType();

            CreateKey(modifierType, polarity, data.TypeData);

            //NOTE:クラス末尾のコメントアウトを参照
            float preMax = Modifiers[modifierType][polarity].Max();
            Modifiers[modifierType][polarity].Add(data.ModifyValue);
            data.AddCallBack(preMax);

            return () => RemoveModifier(data);
        }
        /// <summary>
        /// Modifierを削除する
        /// </summary>
        /// <param name="data">削除するModifier</param>
        void RemoveModifier(ModifierData data)
        {
            Type modifierType = data.ModifierType;
            Type polarity = data.Polarity.GetType();

            //NOTE:クラス末尾のコメントアウトを参照
            Modifiers[modifierType][polarity].Remove(data.ModifyValue);
            data.RemoveCallBack(Modifiers[modifierType][polarity].Max());
        }
        //NOTE:
        /*全てのModifierの処理は追加前の実効値と追加後の実効値を参照できれば十分なはずである、
        (それ以外は重複の効果として無効になっている数値のため、追加前後の実効値の変化が分かれば処理を行うことができる)
        ModifierDataは追加(もしくは削除)する値であるModifyValueを持っているので、
        Addの場合は追加前の効果量最大値、Removeの場合は削除後の効果量最大値を引数として渡せば、
        ModifyValueのデータと合わせることで、preMax値・curMax値を参照できる*/
    }
}