using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public record DispelData;
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
        public abstract Action MakeDispel(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data);
        /// <summary>
        /// それぞれのModifierで固有の解除の条件を定義する
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象者</param>
        /// <returns>解除条件</returns>
        public virtual Observable<Unit> CreateContract(IEntityEnchantInteraction user, IEntityEnchantInteraction target)
        {
            return Observable.Never<Unit>();
        }
    }
    /// <summary>
    /// Modifierの情報
    /// </summary>
    /// <typeparam name="TMod">記録するModifierStockの形式</typeparam>
    public abstract class ModifierBase<TMod> : ModifierBase where TMod : ModifierStock
    {
        public sealed override Action MakeDispel(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data)
        {
            return target.Status.GetModifiers<TMod>().AddModifier(TransformData(user, target, data));
        }
        /// <summary>
        /// ModifierRawDataを加工して、Modifier付与時のCallBackや効果量を固有の処理で付与したものを用意し、Modifierの付与処理へ進む準備をする
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象者</param>
        /// <param name="data">Modifierの情報</param>
        /// <returns>加工されたデータ</returns>
        protected abstract ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data);
    }
    /// <summary>
    /// ステータスに作用するModifierのEnchant方法
    /// </summary>
    /// <typeparam name="TMod">記録するModifierStockの形式</typeparam>
    public abstract class CommonModifier<TMod> : ModifierBase<TMod> where TMod : ModifierParameter
    {
        public abstract StatusType StatusTypeKey { get; }
        protected override ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    if (MathF.Abs(preMax) < Mathf.Abs(data.ModifyValue))
                    {
                        target.Status.GetModifiers<TMod>().ModifierSum[StatusTypeKey] += data.ModifyValue - preMax;
                    }
                    Debug.Log(data.TypeData.Name + ":" + data.ModifyValue + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                curMax =>
                {
                    if (Mathf.Abs(curMax) < Mathf.Abs(data.ModifyValue))
                    {
                        target.Status.GetModifiers<TMod>().ModifierSum[StatusTypeKey] += curMax - data.ModifyValue;
                    }
                    Debug.Log(data.TypeData.Name + ":" + data.ModifyValue + data.TypeData.DisplayUnit + " の効果が解除された。");
                });
        }
    }
    /// <summary>
    /// ステータスに作用しないModifierのEnchant方法
    /// </summary>
    public abstract class TriggerModifier : ModifierBase<TriggerModifierStock> { }
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