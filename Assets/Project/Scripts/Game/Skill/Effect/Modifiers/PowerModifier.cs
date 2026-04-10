using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class PositivePowerRate : PositiveRateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Power;
    }

    [Serializable]
    public class NegativePowerRate : NegativeRateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Power;
    }
}