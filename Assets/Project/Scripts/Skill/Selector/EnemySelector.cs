using System.Collections.Generic;
using UnityEngine;

namespace Skill.Selector
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Skills/Selector/Enemy")]
    public class EnemySelector : SelectorType
    {
        public override IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index)
        {
            return hostile;
        }
    }
}