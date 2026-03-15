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
        record EntityData(IEnumerable<ICombatInteraction> Friendly, IEnumerable<ICombatInteraction> Hostile, int SiblingIndex);
        public string Name { get; init; }
        public int Cost { get; init; }
        public Sprite Icon { get; init; }
        LockOnParameter LockOnParam { get; init; }
        List<EffectData> EffectQue { get; init; }
        Entity Owner { get; init; }
        IEnumerable<ICombatInteraction> targets;
        int queIndex = -1;

        public SkillData(SkillAsset skillAsset, Entity owner)
        {
            Name = skillAsset.SkillName;
            Cost = skillAsset.Cost;
            Icon = skillAsset.Icon;
            LockOnParam = skillAsset.InitialTargeting;
            EffectQue = skillAsset.EffectQue;
            Owner = owner;

            Owner.Targeting.LockOn.Response(res => targets = res.Targets).AddTo(Owner);
        }

        public bool MoveNext()
        {
            if (queIndex < 0)
            {
                Owner.Targeting.LockOn.Call(new((friendly, hostile) => LockOnParam.Selector.Targeting(friendly, hostile, Owner.SiblingIndex).Take(LockOnParam.MaxTargets)));
                queIndex++;
                return MoveNext();
            }
            else
            {
                EffectData current = EffectQue[queIndex];
                if (current.Effect is LockOn targeting)
                {
                    targeting.Apply(Owner, null, current.Parameter);
                    queIndex++;
                    return MoveNext();
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
}