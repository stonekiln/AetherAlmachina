using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ShieldModifier : CommonModifier
    {
        public override StatusType StatusTypeKey => StatusType.Shield;
        public override Action MakeDispel(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            int preMax = target.Status.GetInt(StatusTypeKey);
            Action dispel = target.Status.ModifiedParam[ModifierParameterKey].AddModifier(MakeCommonData(user, target, data));
            int MaxHpDelta = target.Status.GetInt(StatusTypeKey) - preMax;
            Action hpFloor = () =>
            {
                int hpGap = target.Status.GetInt(StatusTypeKey) - target.Status.Resource.Shield;
                if (hpGap < 0)
                {
                    target.Process.ResourceUpdate.Shield.OnNext(new(hpGap));
                }
            };
            switch (MaxHpDelta)
            {
                case > 0:
                    target.Process.ResourceUpdate.Shield.OnNext(new(MaxHpDelta));
                    break;
                case < 0:
                    hpFloor();
                    break;
            }
            return dispel + hpFloor;
        }
    }
    /// <summary>
    /// 定数攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldFlat : ShieldModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// 割合攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldRate : ShieldModifier
    {
        protected override Type ModifierParameterKey => typeof(RateModifierParameter);
        public override string DisplayUnit => "%";
    }
}