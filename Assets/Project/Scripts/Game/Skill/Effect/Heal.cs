using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// 相手に回復を与える効果の定義
    /// </summary>
    public abstract class HealEffectBase : SkillEffect<HealParameter>
    {
        public record UserHealParam(float Recovery, float Power);
        protected void Heal(UserHealParam user, IEntityInteraction target)
        {
            int recovery = Mathf.FloorToInt(user.Recovery * target.Status.Get(StatusType.HealingReceived) * user.Power);
            if ((target.Status.Resource.HitPoint + recovery) >= target.Status.GetInt(StatusType.MaxHitPoint))
            {
                recovery = target.Status.GetInt(StatusType.MaxHitPoint) - target.Status.Resource.HitPoint;
            }
            target.Interaction.ResourceUpdate.HP.Request.OnNext(new(recovery));
            Debug.Log(target.Name + "のHPが" + recovery + "回復しました。\n残りHP:" + target.Status.Resource.HitPoint);
        }
    }
    /// <summary>
    /// 相手に回復を与える効果
    /// </summary>
    [Serializable]
    public class HealEffect : HealEffectBase
    {
        protected override void ApplyTyped(SkillExecutionContext context, HealParameter parameter)
        {
            float healPower = context.User.Status.Get(StatusType.HealPower) * parameter.Power * context.SkillData.HandPower;
            foreach (IEntityInteraction target in context.Targets)
            {
                Heal(new(context.User.Status.Get(StatusType.MaxHitPoint), healPower), target);
            }
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
        [field: SerializeField] public float Power { get; private set; } = 0.1f;
    }
}