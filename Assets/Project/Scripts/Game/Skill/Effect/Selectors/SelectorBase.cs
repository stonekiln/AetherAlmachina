using System.Collections.Generic;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Selectors
{
    public abstract class Selector
    {
        public abstract IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index);
    }
}