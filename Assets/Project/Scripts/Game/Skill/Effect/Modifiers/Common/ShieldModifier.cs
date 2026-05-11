using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ShieldModifier : CommonModifier
    {
        public override StatusType StatusTypeKey => StatusType.Shield;
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        protected override Action MakeDispelTyped(IEntityInteraction user, IEntityInteraction target, CommonModifierData data)
        {
            Action dispel = target.Status.ModifiedParam[ModifierParameterKey].AddModifier(data);
            int valueGap = Mathf.FloorToInt(data.Value) - target.Status.Resource.Shield;
            if (valueGap > 0) target.Process.ResourceUpdate.Shield.OnNext(new(valueGap));

            Action valueMaxFloor = () =>
            {
                int valueGap = target.Status.GetInt(StatusTypeKey) - target.Status.Resource.Shield;
                if (valueGap < 0) target.Process.ResourceUpdate.Shield.OnNext(new(valueGap));
            };
            return dispel + valueMaxFloor;
        }
    }
    /// <summary>
    /// 定数攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldFlat : ShieldModifier
    {
        public override string DisplayUnit => "";

        protected override CommonModifierData MakeModifierData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                StatusTypeKey = StatusTypeKey,
                ModifierType = typeof(ShieldModifier)
            };
        }
    }
    /// <summary>
    /// 割合攻撃力変化のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldHPRate : ShieldModifier
    {
        public override string DisplayUnit => "%";
        protected override CommonModifierData MakeModifierData(IEntityInteraction user, IEntityInteraction target, ModifierRawData data)
        {
            return new(data)
            {
                StatusTypeKey = StatusTypeKey,
                ModifierType = typeof(ShieldModifier),
                Value = user.Status.Get(StatusType.MaxHitPoint) * data.Value / 100f
            };
        }
    }
}