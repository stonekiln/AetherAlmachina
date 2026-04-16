using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class AttackFlat : FlatModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }

    [Serializable]
    public class AttackRate : RateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Attack;
    }
}