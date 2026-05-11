using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class DisableShieldModifier : CommonModifier
    {
        public override StatusType StatusTypeKey => StatusType.Disable;
        public override string DisplayUnit => "回";
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        protected override Action MakeDispelTyped(IEntityInteraction user, IEntityInteraction target, CommonModifierData data)
        {
            Action dispel = target.Status.ModifiedParam[ModifierParameterKey].AddModifier(data);
            int valueGap = Mathf.FloorToInt(data.Value) - target.Status.Resource.Disable;
            if (valueGap > 0) target.Process.ResourceUpdate.Disable.OnNext(new(valueGap));

            Action valueMaxFloor = () =>
            {
                int valueGap = target.Status.GetInt(StatusTypeKey) - target.Status.Resource.Disable;
                if (valueGap < 0) target.Process.ResourceUpdate.Disable.OnNext(new(valueGap));
            };
            return dispel + valueMaxFloor;
        }
    }
}