using System;
using System.Linq;
using Skill.Selector;
using UnityEngine;

namespace Skill.Effects
{
    [CreateAssetMenu(fileName = "Targeting", menuName = "Skills/Effects/Targeting")]
    public class Targeting : SkillEffect<TargetingParameter>
    {
        protected override void ApplyTyped(ICombatInteraction user, ICombatInteraction target, TargetingParameter parameter)
        {
            user.AttackEvent.Targeting.Call(new((friendly, hostile) => parameter.Selector.Targeting(friendly, hostile, user.SiblingIndex).Take(parameter.MaxTargets)));
        }
    }

    [Serializable]
    public class TargetingParameter : EffectParameter
    {
        [field: SerializeField] public SelectorType Selector { get; private set; }
        [field: SerializeField] public int MaxTargets { get; private set; } = 1;
    }
}