using System.Collections.Generic;
using System.Linq;
using R3;
using Skill;
using Skill.Effects;
using UnityEngine;

public class SkillData
{
    record EntityData(IEnumerable<ICombatInteraction> Friendly, IEnumerable<ICombatInteraction> Hostile, int SiblingIndex);
    public string Name { get; init; }
    public int Cost { get; init; }
    public Sprite Icon { get; init; }
    TargetingParameter TargetingParam { get; init; }
    List<EffectData> EffectQue { get; init; }
    Entity Owner { get; init; }
    IEnumerable<ICombatInteraction> targets;
    int queIndex = -1;

    public SkillData(SkillAsset skillAsset, Entity owner)
    {
        Name = skillAsset.SkillName;
        Cost = skillAsset.Cost;
        Icon = skillAsset.Icon;
        TargetingParam = skillAsset.InitialTargeting;
        EffectQue = skillAsset.EffectQue;
        Owner = owner;

        Owner.AttackEvent.Targeting.Response(res => targets = res.Targets).AddTo(Owner);
    }

    public bool MoveNext()
    {
        if (queIndex < 0)
        {
            Owner.AttackEvent.Targeting.Call(new((friendly, hostile) => TargetingParam.Selector.Targeting(friendly, hostile, Owner.SiblingIndex).Take(TargetingParam.MaxTargets)));
            queIndex++;
            return MoveNext();
        }
        else
        {
            EffectData current = EffectQue[queIndex];
            if (current.Effect is Targeting targeting)
            {
                targeting.Apply(Owner, null, current.Parameter);
                queIndex++;
                return MoveNext();
            }
            else
            {
                foreach (ICombatInteraction target in targets)
                {
                    target.AttackEvent.Hit.OnNext(new((entity) => current.Effect.Apply(Owner, entity, current.Parameter)));
                }
                queIndex++;
                return queIndex != EffectQue.Count;
            }
        }
    }
}