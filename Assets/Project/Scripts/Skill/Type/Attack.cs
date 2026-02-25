using UnityEngine;

namespace Skill.Effects
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Skills/Effects")]
    public class AttackEffect : SkillEffect
    {
        public int MaxTargeting = 1;
        public float Power = 1;

        public override void Activate(Entity user, Entity target)
        {
            user.Attack(target, Power);
        }
    }
}