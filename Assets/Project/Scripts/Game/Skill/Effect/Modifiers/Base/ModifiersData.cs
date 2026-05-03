using System.Collections.Generic;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public class ModifiersData
    {
        public FlatModifierParameter Flat { get; init; }
        public RateModifierParameter Rate { get; init; }
        //public TriggerModifierParameter Trigger { get; init; }
        public ModifiersData(Dictionary<StatusType, float> status)
        {
            Flat = new(status);
            Rate = new(status);
        }
    }
}