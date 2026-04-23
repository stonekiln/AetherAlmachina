using System;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class AttackEffect : SkillEffect<AttackParam>
    {
        protected override void ApplyTyped(IEntityInteraction user, IEntityInteraction target, AttackParam parameter)
        {
            user.Command.Attack.OnNext(new((Entity)target, parameter.Power));
        }
    }
    [Serializable]
    public class AttackParam : EffectParameter
    {
        [field: SerializeField] public float Power { get; private set; } = 1f;
    }
}