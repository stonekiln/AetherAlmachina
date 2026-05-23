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

        protected override ModifierData TransformData(IEntityEnchantInteraction user, IEntityEnchantInteraction target, ModifierRawData rawData)
        {
            void KeepRatioHpConvert(float ratio)
            {
                if (ratio != 1)
                {
                    int hpDelta = Mathf.RoundToInt(target.Status.Resource.HitPoint * (ratio - 1f));
                    target.Process.ResourceUpdate.HP.Request.OnNext(new(hpDelta));
                }
            }

            ModifierData data = base.TransformData(user, target, rawData);

            return new(rawData,
                preMax =>
                {
                    float preMaxHP = target.Status.Get(StatusTypeKey);
                    data.AddCallBack(preMax);
                    float curMaxHP = target.Status.Get(StatusTypeKey);

                    KeepRatioHpConvert(curMaxHP / preMaxHP);
                },
                curMax =>
                {
                    float preMaxHP = target.Status.Get(StatusTypeKey);
                    data.RemoveCallBack(curMax);
                    float curMaxHP = target.Status.Get(StatusTypeKey);

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