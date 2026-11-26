using AetherAlmachina.Card.Object;
using AetherAlmachina.Skill;
using DConfig.CardLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using R3;
using System.Collections.Generic;
using System.Linq;

namespace AetherAlmachina.Card.Hand
{
    /// <summary>
    /// カードを画面に表示した状態で制御するためのクラス
    /// </summary>
    public class HandVisualizer : HandController, ILifetimeSpawner
    {
        [Mold("SkillCard/CardBase")]
        Factory<SkillData, CardBase> spawner;

        public override void Injection(InjectableResolver resolver)
        {
            base.Injection(resolver);
        }

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.RegisterPooled<CardLifetime>()
                    .Inject(out spawner);
        }

        protected override List<ICardData> AddHand(List<SkillData> skills)
        {
            return Hand.Concat(skills.Select(skill => spawner.Spawn(skill))).OrderBy(card => card.SkillData.Cost).Select((card, index) => card.SetCard(index)).ToList();
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
