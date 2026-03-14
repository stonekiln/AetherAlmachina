using System;
using UnityEngine;

namespace Skill.Effects
{
    [CreateAssetMenu(fileName = "AttackEffect", menuName = "Skills/Effects/Attack")]
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