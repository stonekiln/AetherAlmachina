using System;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class AttackEffect : SkillEffect<AttackParam>
    {
        protected override void ApplyTyped(ICombatInteraction user, ICombatInteraction target, AttackParam parameter)
        {
            user.Attack((Entity)target, parameter.Power);
        }
    }
    [Serializable]
    public class AttackParam : EffectParameter
    {
        [field: SerializeField] public int Power { get; private set; } = 1;
    }
}