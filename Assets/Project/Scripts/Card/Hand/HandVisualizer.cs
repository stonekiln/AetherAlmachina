using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using DivFacter.Injectable;

/// <summary>
/// カードを画面に表示するためのクラス
/// </summary>
public class HandVisualizer : HandController
{
    Func<SkillData, Transform, CardBase> Create;

    public override void InjectDependencies(InjectableResolver resolver)
    {
        base.InjectDependencies(resolver);
        resolver.Inject(out Create);
    }

    protected override List<ICardData> AddHand(List<SkillData> skills)
    {
        return Hand.Concat(skills.Select(skill => Create(skill, transform))).OrderBy(card => card.SkillData.Cost).Select((card,index) =>card.SetCard(index)).ToList();
    }
    protected override List<ICardData> RemoveHand()
    {
        return Hand.Select(card=>card.IsSelect?card.RemomveCard():card).Where(card=>!card.IsSelect).ToList();
    }
}
