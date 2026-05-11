using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class MaxHitPointModifier : CommonModifier
    {
        public override StatusType StatusTypeKey => StatusType.MaxHitPoint;
        protected override Action MakeDispelTyped(IEntityInteraction user, IEntityInteraction target, CommonModifierData data)
        {
            void keepRatioHpConvert(float ratio)
            {
                if (ratio != 1)
                {
                    int hpDelta = Mathf.RoundToInt(target.Status.Resource.HitPoint * (ratio - 1f));
                    target.Process.ResourceUpdate.HP.OnNext(new(hpDelta));
                }
            }

            float pre = target.Status.Get(StatusTypeKey);
            Action dispel = target.Status.ModifiedParam[ModifierParameterKey].AddModifier(data);
            float cur = target.Status.Get(StatusTypeKey);
            keepRatioHpConvert(cur / pre);

            return () =>
            {
                float pre = target.Status.Get(StatusTypeKey);
                dispel();
                float cur = target.Status.Get(StatusTypeKey);
                keepRatioHpConvert(cur / pre);
            };
        }
    }
    /// <summary>
    /// 定数攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class MaxHitPointFlat : MaxHitPointModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// 割合攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class MaxHitPointRate : MaxHitPointModifier
    {
        protected override Type ModifierParameterKey => typeof(RateModifierParameter);
        public override string DisplayUnit => "%";
    }
}