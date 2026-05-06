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
    /// <summary>
    /// ステータスに作用するModifierのEnchant方法
    /// </summary>
    public abstract class CommonModifier : ModifierBase
    {
        /// <summary>
        /// Modifierの変化の種類
        /// </summary>
        protected abstract Type ModifierParameterKey { get; }
        public abstract StatusType StatusTypeKey { get; }
        public override Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return target.Status.ModifiedParam[ModifierParameterKey].AddModifier(MakeCommonData(user, target, data));
        }
        protected abstract CommonModifierData MakeCommonData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data);
    }
    public abstract class TriggerModifier : ModifierBase
    {
        public override Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return target.Status.Triggers.AddModifier(MakeTriggerData(user, target, data));
        }
        protected abstract TriggerModifierData MakeTriggerData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data);
    }
    /// <summary>
    /// CommonModifierの基本的な付与方法
    /// </summary>
    public abstract class BasicCommonModifier : CommonModifier
    {
        protected override CommonModifierData MakeCommonData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                StatusTypeKey = StatusTypeKey
            };
        }
    }
    /// <summary>
    /// 定数変化のModifierの定義
    /// </summary>
    public abstract class FlatModifier : BasicCommonModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// 割合変化のModifierの定義
    /// </summary>
    public abstract class RateModifier : BasicCommonModifier
    {
        protected override Type ModifierParameterKey => typeof(RateModifierParameter);
        public override string DisplayUnit => "%";
    }
}