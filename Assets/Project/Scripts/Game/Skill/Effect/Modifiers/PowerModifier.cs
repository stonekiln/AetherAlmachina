using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class PowerModifier : PositiveRateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Power;
    }
}