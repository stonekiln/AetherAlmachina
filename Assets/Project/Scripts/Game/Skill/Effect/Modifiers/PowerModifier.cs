using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public class PowerModifier : PercentModifier
    {
        protected override StatusType StatusTypeKey => StatusType.Power;
    }
}