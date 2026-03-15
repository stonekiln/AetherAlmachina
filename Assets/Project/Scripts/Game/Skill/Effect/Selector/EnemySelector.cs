using System.Collections.Generic;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Selector
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Skills/Selector/Enemy")]
    public class EnemySelector : SelectorBase
    {
        public override IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index)
        {
            return hostile;
        }
    }
}