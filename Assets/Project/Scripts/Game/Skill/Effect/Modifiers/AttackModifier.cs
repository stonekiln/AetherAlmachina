using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class AttackFlat : PositiveFlatModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }

    [Serializable]
    public class AttackPercent : PositiveRateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }
}