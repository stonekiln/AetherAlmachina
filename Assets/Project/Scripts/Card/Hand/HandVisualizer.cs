using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using DivFacter.Injectable;
using DivFacter.Lifetime;
using System.IO;
using VContainer;
using DConfig.CardLife;

/// <summary>
/// カードを画面に表示するためのクラス
/// </summary>
public class HandVisualizer : HandController, ILifetimeSpawner
{
    Func<SkillData, CardBase> Create;

    public override void InjectDependencies(InjectableResolver resolver)
    {
        base.InjectDependencies(resolver);
    }

    public void SpawnConfigure(ObjectBuilder builder)
    {
        builder.Set<CardLifetimeScope, SkillData, CardBase>(out Create, Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")),
        (data, resolver) =>
        {
            CardBase cardBase = resolver.Resolve<CardBase>().Initialize(data);
        });
    }

    protected override List<ICardData> AddHand(List<SkillData> skills)
    {
        return Hand.Concat(skills.Select(skill => Create(skill))).OrderBy(card => card.SkillData.Cost).Select((card, index) => card.SetCard(index)).ToList();
    }
    protected override List<ICardData> RemoveHand()
    {
        return Hand.Select(card => card.IsSelect ? card.RemoveCard() : card).Where(card => !card.IsSelect).ToList();
    }
}
