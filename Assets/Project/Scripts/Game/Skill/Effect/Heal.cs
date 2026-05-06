using System;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// 相手に回復を与える効果
    /// </summary>
    [Serializable]
    public class HealEffect : SkillEffect<HealParameter>
    {
        protected override void ApplyTyped(IEntityInteraction user, IEntityInteraction target, HealParameter parameter)
        {
            user.Action.Heal.Activation.OnNext(new(target, parameter.Power));
        }
    }
    /// <summary>
    /// HealEffectに必要なパラメータ
    /// </summary>
    [Serializable]
    public class HealParameter : EffectParameter
    {
        /// <summary>
        /// 回復スキルの威力
        /// </summary>
        [field: SerializeField] public float Power { get; private set; } = 1f;
    }
}