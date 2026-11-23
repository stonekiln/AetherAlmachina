using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using R3;
using DConfig.CardLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using AetherAlmachina.Card.Object;
using AetherAlmachina.Skill;

namespace AetherAlmachina.Card.Hand
{
    /// <summary>
    /// カードを画面に表示した状態で制御するためのクラス
    /// </summary>
    public class HandVisualizer : HandController, ILifetimeSpawner
    {
        Func<SkillData, CardBase> spawner;

        public override void Injection(InjectableResolver resolver)
        {
            base.Injection(resolver);
        }

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.RegisterPooled<CardLifetime>(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")))
                    .Inject(out spawner);
        }

        protected override List<ICardData> AddHand(List<SkillData> skills)
        {
            return Hand.Concat(skills.Select(skill => spawner(skill))).OrderBy(card => card.SkillData.Cost).Select((card, index) => card.SetCard(index)).ToList();
        }
        protected override List<ICardData> RemoveHand()
        {
            // 返却で選択状態がリセットされる前に、残すカードを確定する。
            List<ICardData> remaining = new();
            foreach (ICardData card in Hand)
            {
                if (card.IsSelect) card.RemoveCard();
                else remaining.Add(card);
            }
            return remaining;
        }
    }
}
