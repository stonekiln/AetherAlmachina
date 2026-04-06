using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class AttackFlat : FlatModifier
    {
        protected override StatusType StatusTypeKey => StatusType.Attack;
    }

    [Serializable]
    public class AttackPercent : PercentModifier
    {
        protected override StatusType StatusTypeKey => StatusType.Attack;
    }
}