using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill
{
    public class SkillData
    {
        record EntityData(IEnumerable<ICombatInteraction> Friendly, IEnumerable<ICombatInteraction> Hostile, int SiblingIndex);
        public string Name { get; init; }
        public int Cost { get; init; }
        public Sprite Icon { get; init; }
        List<EffectData> EffectQue { get; init; }
        Entity Owner { get; init; }
        IEnumerable<ICombatInteraction> targets;
        int queIndex = 0;

        public SkillData(SkillAsset skillAsset, Entity owner)
        {
            Name = skillAsset.SkillName;
            Cost = skillAsset.Cost;
            Icon = skillAsset.Icon;
            EffectQue = new() { new(Activator.CreateInstance<LockOn>(), skillAsset.InitialTargeting) };
            EffectQue.AddRange(skillAsset.EffectQue);
            Owner = owner;

            Owner.Targeting.LockOn.Response(res => targets = res.Targets).AddTo(Owner);
        }

        public bool MoveNext()
        {
            EffectData current = EffectQue[queIndex];
            if (current.Effect is LockOn targeting)
            {
                targeting.Apply(Owner, null, current.Parameter);
                queIndex++;
                return queIndex != EffectQue.Count && MoveNext();
            }
            else
            {
                foreach (ICombatInteraction target in targets)
                {
                    target.Targeting.Hit.OnNext(new((entity) => current.Effect.Apply(Owner, entity, current.Parameter)));
                }
                queIndex++;
                return queIndex != EffectQue.Count;
            }
        }
    }
}