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
        public string Name { get; init; }
        public int Cost { get; init; }
        public Sprite Icon { get; init; }
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
            EffectQueue = new[] { new EffectData(Activator.CreateInstance(typeof(LockOn)) as LockOn, skillAsset.InitialLockOn) }.Concat(skillAsset.EffectQueue).ToArray();
            Owner = owner;
            Owner.Targeting.LockOn.Response(res => targets = res.Targets).AddTo(Owner);
        }

        public bool MoveNext()
        {
            EffectData current = EffectQueue[queueIndex];
            if (current.Effect is LockOn targeting)
            {
                targeting.Apply(Owner, null, current.Parameter);
                queueIndex++;
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