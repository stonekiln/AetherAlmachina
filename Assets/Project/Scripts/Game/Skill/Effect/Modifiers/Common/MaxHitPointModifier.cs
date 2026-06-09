using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// 最大HP変化のModifierの定義
    /// </summary>
    /// <typeparam name="TMod">記録するModifierStockの形式</typeparam>
    public abstract class MaxHitPointModifier<TMod> : CommonModifier<TMod> where TMod : ModifierParameter
    {
        public override StatusType StatusTypeKey => StatusType.MaxHitPoint;

        protected override ModifierData TransformData(EnchantExecutionContext context, ModifierRawData data)
        {
            void KeepRatioHpConvert(float ratio)
            {
                if (ratio != 1)
                {
                    int hpDelta = Mathf.RoundToInt(context.Target.Status.Resource.HitPoint * (ratio - 1f));
                    context.Target.Interaction.ResourceUpdate.HP.Request.OnNext(new(hpDelta));
                }
            }

            ModifierData newData = base.TransformData(context, data);

            return new(data,
                preMax =>
                {
                    float preMaxHP = context.Target.Status.Get(StatusTypeKey);
                    newData.AddCallBack(preMax);
                    float curMaxHP = context.Target.Status.Get(StatusTypeKey);

                    KeepRatioHpConvert(curMaxHP / preMaxHP);
                },
                curMax =>
                {
                    float preMaxHP = context.Target.Status.Get(StatusTypeKey);
                    newData.RemoveCallBack(curMax);
                    float curMaxHP = context.Target.Status.Get(StatusTypeKey);

                    KeepRatioHpConvert(curMaxHP / preMaxHP);
                }
            );
        }
    }
}
/// <summary>
/// 定数最大HP変化のModifierの定義
/// </summary>
[Serializable]
public class MaxHitPointFlat : MaxHitPointModifier<FlatModifierParameter>
{
    public override string DisplayUnit => "";
}
/// <summary>
/// 割合最大HP変化のModifierの定義
/// </summary>
[Serializable]
public class MaxHitPointRate : MaxHitPointModifier<RateModifierParameter>
{
    public override string DisplayUnit => "%";
}