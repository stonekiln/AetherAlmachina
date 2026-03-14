using System.Collections.Generic;
using UnityEngine;

namespace Skill.Selector
{
    public abstract class SelectorType : ScriptableObject
    {
        public abstract IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index);
    }
}