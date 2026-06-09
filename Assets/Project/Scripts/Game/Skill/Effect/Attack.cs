using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using Tools.Helpers;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// 相手にダメージを与える効果の定義
    /// </summary>
    public abstract class AttackEffectBase : SkillEffect<AttackParameter>
    {
        public record UserAttackParam(float Attack, float Power);
        protected void Attack(UserAttackParam user, IEntityInteraction target)
        {
            int damage = Mathf.FloorToInt((user.Attack - target.Status.Get(StatusType.Defence)) * user.Power * target.Status.Get(StatusType.DamageTaken));
            if (damage < 0) damage = 0;
            if (target.Status.Resource.Disable > 0)
            {
                target.Interaction.ResourceUpdate.Disable.Request.OnNext(new(-1));
                Debug.Log(target.Name + "が攻撃を回避した。\n残り回数:" + target.Status.Resource.Disable);
                damage = 0;
            }
            else if (target.Status.Resource.Shield > 0)
            {
                int shieldDamage;
                if (target.Status.Resource.Shield > damage)
                {
                    shieldDamage = damage;
                }
                else
                {
                    shieldDamage = target.Status.Resource.Shield;
                }
                damage -= shieldDamage;
                target.Interaction.ResourceUpdate.Shield.Request.OnNext(new(-shieldDamage));
                Debug.Log(target.Name + "が" + shieldDamage + "のシールドを消費しました\n残りシールド:" + target.Status.Resource.Shield);

            }
            if (damage == 0)
            {
                Debug.Log(target.Name + "がダメージを無効化しました。\n現在HP:" + target.Status.Resource.HitPoint);
            }
            else
            {
                target.Interaction.ResourceUpdate.HP.Request.OnNext(new(-damage));
                Debug.Log(target.Name + "が" + damage + "ダメージを受けました。\n残りHP:" + target.Status.Resource.HitPoint);
            }
        }
    }
    /// <summary>
    /// 相手にダメージを与える効果
    /// </summary>
    [Serializable]
    public class AttackEffect : AttackEffectBase
    {
        protected override void ApplyTyped(SkillExecutionContext context, AttackParameter parameter)
        {
            float attackerPower = context.User.Status.Get(StatusType.Power) * parameter.Power * context.SkillData.HandPower;
            if (Probability.Try(context.User.Status.Get(StatusType.CriticalRate)))
            {
                Debug.Log("クリティカルが発生しました。");
                attackerPower *= context.User.Status.Get(StatusType.CriticalDamage);
            }

            foreach (IEntityInteraction target in context.Targets)
            {
                Attack(new(context.User.Status.Get(StatusType.Attack), attackerPower), target);
            }
        }
    }

    /// <summary>
    /// AttackEffectに必要なパラメータ
    /// </summary>
    [Serializable]
    public class AttackParameter : EffectParameter
    {
        /// <summary>
        /// 攻撃スキルの威力
        /// </summary>
        [field: SerializeField] public float Power { get; private set; } = 1f;
    }
}