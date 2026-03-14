using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Skill.Selector
{
    [CreateAssetMenu(fileName = "Self", menuName = "Skills/Selector/Self")]
    public class SelfSelector : SelectorType
    {
        public override IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index)
        {
            List<ICombatInteraction> list = friendly.ToList();
            (list[0], list[index]) = (list[index], list[0]);
            return list;
        }
    }
}