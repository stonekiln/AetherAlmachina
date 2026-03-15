using System.Collections.Generic;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Selector
{
    public abstract class SelectorBase : ScriptableObject
    {
        public abstract IEnumerable<ICombatInteraction> Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index);
    }
}