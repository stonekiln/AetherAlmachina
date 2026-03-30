using System;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Selector;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class LockOn : SkillEffect<LockOnParameter>
    {
        protected override void ApplyTyped(ICombatInteraction user, ICombatInteraction target, LockOnParameter parameter)
        {
            user.Targeting.LockOn.Call(new((friendly, hostile) => parameter.Selector.Targeting(friendly, hostile, user.SiblingIndex).Take(parameter.MaxTargets)));
        }
    }

    [Serializable]
    public class LockOnParameter : EffectParameter
    {
        [field: SerializeReference] public SelectorBase Selector { get; private set; }
        [field: SerializeField] public int MaxTargets { get; private set; } = 1;
    }
}