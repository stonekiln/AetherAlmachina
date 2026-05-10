using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class MaxHitPointModifier : CommonModifier
    {
        public override StatusType StatusTypeKey => StatusType.MaxHitPoint;
        public override Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            float pre = target.Status.Get(StatusTypeKey);
            Action dispel = target.Status.ModifiedParam[ModifierParameterKey].AddModifier(MakeCommonData(user, target, data));
            float cur = target.Status.Get(StatusTypeKey);
            if (cur != pre)
            {
                float ratio = cur / pre;
                int hpDelta = Mathf.FloorToInt(target.Status.Resource.HitPoint * ratio) - target.Status.Resource.HitPoint;
                target.Process.ResourceUpdate.HP.OnNext(new(hpDelta));
            }

            return () =>
            {
                float pre = target.Status.Get(StatusTypeKey);
                dispel();
                float cur = target.Status.Get(StatusTypeKey);
                if (cur != pre)
                {
                    float ratio = cur / pre;
                    int hpDelta = Mathf.CeilToInt(target.Status.Resource.HitPoint * ratio) - target.Status.Resource.HitPoint;
                    target.Process.ResourceUpdate.HP.OnNext(new(hpDelta));
                }
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