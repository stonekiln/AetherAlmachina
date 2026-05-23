using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
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
                    target.Process.ResourceUpdate.HP.OnNext(new(hpDelta));
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
/// 定数攻撃力変化のModifierの定義
/// </summary>
[Serializable]
public class MaxHitPointFlat : MaxHitPointModifier<FlatModifierParameter>
{
    public override string DisplayUnit => "";
}
/// <summary>
/// 割合攻撃力変化のModifierの定義
/// </summary>
[Serializable]
public class MaxHitPointRate : MaxHitPointModifier<RateModifierParameter>
{
    public override string DisplayUnit => "%";
}