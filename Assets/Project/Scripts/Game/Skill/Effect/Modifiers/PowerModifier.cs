using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class PowerRate : RateModifier
    {
        public override StatusType StatusTypeKey => StatusType.Power;
    }
}