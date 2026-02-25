using System.Collections.Generic;
using System.Linq;
using Skill.Effects;
using UnityEngine;

public class SkillData
{
    record EntityData(IEnumerable<ICombatInteraction> Friendly, IEnumerable<ICombatInteraction> Hostile, int SiblingIndex);
    public string Name { get; init; }
    public int Cost { get; init; }
    public Sprite Icon { get; init; }
    Targeting InitialTargeting { get; init; }
    List<SkillEffect> EffectQue { get; init; }
    Entity Owner { get; init; }
    EntityData entityData;
    IEnumerable<ICombatInteraction> targets;
    int queIndex;

    public SkillData(SkillAsset skillAsset, Entity owner)
    {
        Name = skillAsset.SkillName;
        Cost = skillAsset.Cost;
        Icon = skillAsset.Icon;
        InitialTargeting = skillAsset.InitialTargeting;
        EffectQue = skillAsset.EffectQue;
        Owner = owner;
    }
    public void Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile, int index)
    {
        entityData = new(friendly, hostile, index);
        targets = InitialTargeting.Activate(friendly, hostile, index);
        queIndex = 0;
    }
    public void MoveNext()
    {
        if (EffectQue[queIndex] is Targeting targeting)
        {
            targets = targeting.Activate(entityData.Friendly, entityData.Hostile, entityData.SiblingIndex);
            queIndex++;
            MoveNext();
        }
        else
        {
            foreach (ICombatInteraction target in targets)
            {
                target.AttackEvent.Hit.OnNext(new((entity) => EffectQue[queIndex].Activate(Owner, entity)));
            }
            queIndex++;
        }
    }
}