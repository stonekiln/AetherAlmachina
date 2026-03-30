using System.Collections.Generic;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Selector
{
    public abstract class SelectorBase
    {
        public abstract IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index);
    }
}