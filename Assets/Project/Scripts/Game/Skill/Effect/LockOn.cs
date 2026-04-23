using System;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Selectors;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class LockOn : SkillEffect<LockOnParameter>
    {
        protected override void ApplyTyped(IEntityInteraction user, IEntityInteraction target, LockOnParameter parameter)
        {
            user.Targeting.LockOn.Call(new((friendly, hostile) => parameter.Selector.Targeting(friendly, hostile, user.SiblingIndex).Take(parameter.MaxTargets)));
        }
    }

    [Serializable]
    public class LockOnParameter : EffectParameter
    {
        [field: SerializeReference] public Selector Selector { get; private set; }
        [field: SerializeField] public int MaxTargets { get; private set; } = 1;
    }
}