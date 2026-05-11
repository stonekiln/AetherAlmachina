using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの情報
    /// </summary>
    public abstract class ModifierBase
    {
        public abstract string DisplayUnit { get; }

        /// <summary>
        /// Modifierを付与する
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象</param>
        /// <param name="modifierData">Modifierの情報</param>
        /// <returns>解除を行うための情報</returns>
        public abstract Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data);
    }
    public abstract class ModifierBase<TData> : ModifierBase where TData : ModifierRawData
    {
        public sealed override Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return MakeDispelTyped(user, target, MakeModifierData(user, target, data));
        }
        protected abstract Action MakeDispelTyped(IEntityInteraction user, IEntityInteraction target, TData data);
        protected abstract TData MakeModifierData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data);
    }
    /// <summary>
    /// ステータスに作用するModifierのEnchant方法
    /// </summary>
    public abstract class CommonModifier : ModifierBase<CommonModifierData>
    {
        /// <summary>
        /// Modifierの変化の種類
        /// </summary>
        protected abstract Type ModifierParameterKey { get; }
        public abstract StatusType StatusTypeKey { get; }
        protected override Action MakeDispelTyped(IEntityInteraction user, IEntityInteraction target, CommonModifierData data)
        {
            return target.Status.ModifiedParam[ModifierParameterKey].AddModifier(data);
        }
        protected override CommonModifierData MakeModifierData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                StatusTypeKey = StatusTypeKey
            };
        }
    }
    public abstract class TriggerModifier : ModifierBase<TriggerModifierData>
    {
        protected override Action MakeDispelTyped(IEntityInteraction user, IEntityInteraction target, TriggerModifierData data)
        {
            return target.Status.Triggers.AddModifier(data);
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