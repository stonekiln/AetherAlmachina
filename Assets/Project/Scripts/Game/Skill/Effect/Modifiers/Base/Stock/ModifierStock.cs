using System;
using System.Collections.Generic;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ModifierStock
    {
        protected Dictionary<Type, Dictionary<Type, ModifierValues>> Modifiers { get; init; }
    }
    public abstract class ModifierStock<TData> : ModifierStock where TData : ModifierRawData
    {
        /// <summary>
        /// Modifierを追加する
        /// </summary>
        /// <param name="data">追加するModifier</param>
        /// <returns>自身のmodifierを解除するための関数</returns>
        public abstract Action AddModifier(TData data);
        /// <summary>
        /// Modifierを削除する
        /// </summary>
        /// <param name="data">削除するModifier</param>
        protected abstract void RemoveModifier(TData data);
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
    }
}