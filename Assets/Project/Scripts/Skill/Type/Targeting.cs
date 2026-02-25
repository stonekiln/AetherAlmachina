using System.Collections.Generic;

namespace Skill.Effects
{
    public abstract class Targeting : SkillEffect
    {
        public abstract IEnumerable<ICombatInteraction> Activate(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index);
    }
}