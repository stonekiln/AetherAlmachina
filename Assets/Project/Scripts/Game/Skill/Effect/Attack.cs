using System;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// 相手にダメージを与える効果
    /// </summary>
    [Serializable]
    public class AttackEffect : SkillEffect<AttackParam>
    {
        protected override void ApplyTyped(IEntityInteraction user, IEntityInteraction target, AttackParam parameter)
        {
            user.Command.Attack.OnNext(new((Entity)target, parameter.Power));
        }
    }
    /// <summary>
    /// AttackEffectに必要なパラメータ
    /// </summary>
    [Serializable]
    public class AttackParam : EffectParameter
    {
        /// <summary>
        /// 攻撃スキルの威力
        /// </summary>
        [field: SerializeField] public float Power { get; private set; } = 1f;
    }
}