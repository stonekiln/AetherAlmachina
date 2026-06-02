using System;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Selectors;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// LockOn専用のSkillEffect(targetを引数に取らない)
    /// </summary>
    public interface ILockOnEffect
    {
        /// <summary>
        /// スキルの効果を実行する(LockOn専用)
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="parameter">エフェクトの設定値</param>
        void Apply(Entity user, EffectParameter parameter);
    }
    /// <summary>
    /// スキルエフェクトの効果を及ぼす対象を抽出する
    /// </summary>
    [Serializable]
    public class LockOn : SkillEffect, ILockOnEffect
    {
        public override Type ParameterType => typeof(LockOnParameter);

        public void Apply(Entity user, EffectParameter parameter)
        {
            ApplyTyped(user, (LockOnParameter)parameter);
        }
        public override void Apply(Entity user, Entity target, EffectParameter parameter)
        {
            Apply(user, (LockOnParameter)parameter);
        }
        void ApplyTyped(IEntityInteraction user, LockOnParameter parameter)
        {
            user.Targeting.LockOn.Request.OnNext(new((friendly, hostile) =>
                parameter.Selector.Targeting(friendly, hostile, user.LayoutIndex).Take(parameter.MaxTargets)));
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