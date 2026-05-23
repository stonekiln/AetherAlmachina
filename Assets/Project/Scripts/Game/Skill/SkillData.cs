using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill
{
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
        /// 遅延するべきスキルかどうか
        /// </summary>
        public bool IsDeferrable { get; init; }
        /// <summary>
        /// スキルの使用者
        /// </summary>
        public IEntityInteraction User => Owner;
        protected Entity Owner { get; init; }
        protected EffectData[] EffectQueue { get; init; }

        public SkillData(SkillAsset skillAsset, Entity owner)
        {
            Name = skillAsset.SkillName;
            Cost = skillAsset.Cost;
            Icon = skillAsset.Icon;
            IsDeferrable = skillAsset.InitialLockOn.Selector.IsDeferrable;
            Owner = owner;
            //0番目にInitialLockOnを置いたEffectQueueの配列を渡す
            EffectQueue = new[] { new EffectData(Activator.CreateInstance(typeof(LockOn)) as LockOn, skillAsset.InitialLockOn) }.Concat(skillAsset.EffectQueue).ToArray();
        }
        public SkillData(SkillData data)
        {
            Name = data.Name;
            Cost = data.Cost;
            Icon = data.Icon;
            IsDeferrable = data.IsDeferrable;
            Owner = data.Owner;
            EffectQueue = data.EffectQueue;
        }
    }
    /// <summary>
    /// スキルの情報を渡す
    /// </summary>
    public class ActivatedSkillData : SkillData
    {
        float HandPower { get; init; }
        IEnumerable<Entity> targets;
        int queueIndex = 0;

        public ActivatedSkillData(SkillData data, float power) : base(data)
        {
            HandPower = power;
            Owner.Targeting.LockOn.Response.Subscribe(log => targets = log.Targets.OfType<Entity>()).AddTo(Owner);
        }

        /// <summary>
        /// 次のエフェクトを取り出す
        /// </summary>
        /// <returns>効果が終了したかどうか</returns>
        public bool MoveNext()
        {
            EffectData current = EffectQueue[queueIndex];
            //HACK:LockOnだった場合targetを必要としないため少し特殊な処理を行う
            // LockOnにキャストを行ってApplyを実行することもできるが、
            // インターフェイスにキャストを行って、インターフェイス内にあるtargetを必要としないメソッドから実行する
            if (current.Effect is ILockOnEffect targeting)
            {
                targeting.Apply(Owner, current.Parameter);
                queueIndex++;
                //LockOnだった場合それはモーションを伴わないので自動で次の効果を発動させる
                return queueIndex != EffectQueue.Length && MoveNext();
            }
            else
            {
                foreach (Entity target in targets)
                {
                    target.Targeting.Hit.OnNext(new(entity => current.Effect.Apply(Owner, entity, current.Parameter.SetHandPower(HandPower))));
                }
                queueIndex++;
                return queueIndex != EffectQueue.Length;
            }
        }
    }
}