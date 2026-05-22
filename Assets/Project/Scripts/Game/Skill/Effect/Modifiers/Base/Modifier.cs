using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

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
        /// <returns>解除を行うための動作</returns>
        public abstract Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data);
    }
    /// <summary>
    /// Modifierの情報
    /// </summary>
    /// <typeparam name="TMod">Modifierの形式</typeparam>
    public abstract class ModifierBase<TMod, TData> : ModifierBase
        where TMod : ModifierStock<TData>
        where TData : ModifierRawData
    {
        public sealed override Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return target.Status.GetModifiers<TMod>().AddModifier(TransformData(user, target, data));
        }
        protected abstract TData TransformData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data);
    }
    /// <summary>
    /// ステータスに作用するModifierのEnchant方法
    /// </summary>
    public abstract class CommonModifier<TMod> : ModifierBase<TMod, CommonModifierData> where TMod : ModifierParameter
    {
        public abstract StatusType StatusTypeKey { get; }
        protected override CommonModifierData TransformData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                StatusTypeKey = StatusTypeKey
            };
        }
    }
    /// <summary>
    /// ステータスに作用しないModifierのEnchant方法
    /// </summary>
    public abstract class TriggerModifier : ModifierBase<TriggerModifiers, TriggerModifierData>
    {
        protected override TriggerModifierData TransformData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                AddCallBack = delegate { },
                RemoveCallBack = delegate { }
            };
        }
    }
    /// <summary>
    /// 定数変化のModifierの定義
    /// </summary>
    public abstract class FlatModifier : CommonModifier<FlatModifierParameter>
    {
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// 割合変化のModifierの定義
    /// </summary>
    public abstract class RateModifier : CommonModifier<RateModifierParameter>
    {
        public override string DisplayUnit => "%";
    }
}