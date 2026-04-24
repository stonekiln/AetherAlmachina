using System;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Selectors;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// スキルエフェクトの効果を及ぼす対象を抽出する
    /// </summary>
    [Serializable]
    public class LockOn : SkillEffect<LockOnParameter>
    {
        protected override void ApplyTyped(IEntityInteraction user, IEntityInteraction target, LockOnParameter parameter)
        {
            user.Targeting.LockOn.Call(new((friendly, hostile) => parameter.Selector.Targeting(friendly, hostile, user.SiblingIndex).Take(parameter.MaxTargets)));
        }
    }
    /// <summary>
    /// LockOnに必要なパラメータ
    /// </summary>
    [Serializable]
    public class LockOnParameter : EffectParameter
    {
        /// <summary>
        /// 対象の抽出方法
        /// </summary>
        [field: SerializeReference] public Selector Selector { get; private set; }
        /// <summary>
        /// 抽出できる最大値
        /// </summary>
        [field: SerializeField] public int MaxTargets { get; private set; } = 1;
    }
}