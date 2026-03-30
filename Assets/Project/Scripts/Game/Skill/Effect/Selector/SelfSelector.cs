using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Selector
{
    [Serializable]
    public class SelfSelector : SelectorBase
    {
        public override IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index)
        {
            List<ICombatInteraction> list = friendly.ToList();
            (list[0], list[index]) = (list[index], list[0]);
            return list;
        }
    }
}