using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class PositiveAttackFlat : PositiveFlatModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }

    [Serializable]
    public class PositiveAttackRate : PositiveRateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }

    [Serializable]
    public class NegativeAttackFlat : NegativeFlatModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }

    [Serializable]
    public class NegativeAttackRate : NegativeRateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }
}