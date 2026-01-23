using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using R3;
using DConfig.CardLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;

/// <summary>
/// カードを画面に表示するためのクラス
/// </summary>
public class HandVisualizer : HandController, ILifetimeSpawner
{
    Func<SkillData, CardBase> spawner;

    public override void InjectDependencies(InjectableResolver resolver)
    {
        base.InjectDependencies(resolver);
    }

    public void SpawnConfigure(SpawnerBuilder builder)
    {
        builder.Register<CardLifetime>(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")))
                .Inject(out spawner);
    }

    protected override List<ICardData> AddHand(List<SkillData> skills)
    {
        return Hand.Concat(skills.Select(skill => spawner(skill))).OrderBy(card => card.SkillData.Cost).Select((card, index) => card.SetCard(index)).ToList();
    }
    protected override List<ICardData> RemoveHand()
    {
        return Hand.Select(card => card.IsSelect ? card.RemoveCard() : card).Where(card => !card.IsSelect).ToList();
    }
}
