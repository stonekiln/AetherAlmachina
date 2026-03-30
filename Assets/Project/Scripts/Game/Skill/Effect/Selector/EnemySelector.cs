using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Selector
{
    [Serializable]
    public class EnemySelector : SelectorBase
    {
        public override IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index)
        {
            return hostile;
        }
    }
}