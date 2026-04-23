using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの単位を表すインターフェイス
    /// </summary>
    public interface IModifierUnit
    {
        /// <summary>
        /// Modifierの単位を表す
        /// </summary>
        public string DisplayUnit { get; }
    }
    /// <summary>
    /// Modifierの情報
    /// </summary>
    public abstract class ModifierBase : IModifierUnit
    {
        public abstract string DisplayUnit { get; }
        /// <summary>
        /// Modifierの変化の種類
        /// </summary>
        protected abstract Type ModifierParameterKey { get; }
        /// <summary>
        /// 変化するステータス
        /// </summary>
        public abstract StatusType StatusTypeKey { get; }

        /// <summary>
        /// Modifierを付与する対象を決める
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象候補</param>
        /// <param name="modifierData">Modifierの情報</param>
        /// <returns>解除を行うための情報</returns>
        public abstract DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target, IModifierData modifierData);
    }
    /// <summary>
    /// 基本的なEnchant方法のModifier
    /// </summary>
    public abstract class CommonModifier : ModifierBase
    {
        public override DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target, IModifierData modifierData)
        {
            Action remove = target.Status.Modifiers[ModifierParameterKey].AddModifier(modifierData);
            return new(user, target, remove);
        }
    }
    /// <summary>
    /// 定数変化のModifierの定義
    /// </summary>
    public abstract class FlatModifier : CommonModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// 割合変化のModifierの定義
    /// </summary>
    public abstract class RateModifier : CommonModifier
    {
        protected override Type ModifierParameterKey => typeof(RateModifierParameter);
        public override string DisplayUnit => "%";
    }
}