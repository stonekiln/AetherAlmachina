using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの情報
    /// </summary>
    public abstract class ModifierBase
    {
        public abstract string DisplayUnit { get; }
        /// <summary>
        /// Modifierの変化の種類
        /// </summary>
        protected abstract Type ModifierParameterKey { get; }

        /// <summary>
        /// Modifierを付与する対象を決める
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象候補</param>
        /// <param name="modifierData">Modifierの情報</param>
        /// <returns>解除を行うための情報</returns>
        public abstract DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target, ModifierEnchantData modifierData);
    }
    public abstract class TriggerModifier : ModifierBase
    {
        public override DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target, ModifierEnchantData modifierData)
        {
            return EnchantTyped(user, target, new(modifierData));
        }
        protected abstract DispelModifier EnchantTyped(IEntityInteraction user, IEntityInteraction target, TriggerModifierData modifierData);
    }
    /// <summary>
    /// 基本的なEnchant方法のModifier
    /// </summary>
    public abstract class CommonModifier : ModifierBase
    {
        public abstract StatusType StatusTypeKey { get; }
        public override DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target, ModifierEnchantData modifierData)
        {
            return EnchantTyped(user, target, new(modifierData));
        }
        DispelModifier EnchantTyped(IEntityInteraction user, IEntityInteraction target, CommonModifierData modifierData)
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