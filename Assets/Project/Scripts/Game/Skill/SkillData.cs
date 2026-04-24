using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill
{
    /// <summary>
    /// スキルの情報を渡す
    /// </summary>
    public class SkillData
    {
        /// <summary>
        /// スキル名
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// スキルの発動コスト
        /// </summary>
        public int Cost { get; init; }
        /// <summary>
        /// スキルアイコン
        /// </summary>
        public Sprite Icon { get; init; }
        /// <summary>
        /// スキルの使用者
        /// </summary>
        public IEntityInteraction User => Owner;
        EffectData[] EffectQueue { get; init; }
        Entity Owner { get; init; }
        IEnumerable<IEntityInteraction> targets;
        int queueIndex = 0;

        public SkillData(SkillAsset skillAsset, Entity owner)
        {
            Name = skillAsset.SkillName;
            Cost = skillAsset.Cost;
            Icon = skillAsset.Icon;
            //0番目にInitialLockOnを置いたEffectQueueの配列を渡す
            EffectQueue = new[] { new EffectData(Activator.CreateInstance(typeof(LockOn)) as LockOn, skillAsset.InitialLockOn) }.Concat(skillAsset.EffectQueue).ToArray();
            Owner = owner;
            Owner.Targeting.LockOn.Response(res => targets = res.Targets).AddTo(Owner);
        }
        /// <summary>
        /// 次のエフェクトを取り出す
        /// </summary>
        /// <returns>効果が終了したかどうか</returns>
        public bool MoveNext()
        {
            EffectData current = EffectQueue[queueIndex];
            //LockOnだった場合通常とは異なる処理を行う
            if (current.Effect is LockOn targeting)
            {
                targeting.Apply(Owner, null, current.Parameter);
                queueIndex++;
                //LockOnだった場合それはモーションを伴わないので自動で次の効果を発動させる
                return queueIndex != EffectQueue.Length && MoveNext();
            }
            else
            {
                foreach (IEntityInteraction target in targets)
                {
                    target.Targeting.Hit.OnNext(new(entity => current.Effect.Apply(Owner, entity, current.Parameter)));
                }
                queueIndex++;
                return queueIndex != EffectQueue.Length;
            }
        }
    }
}