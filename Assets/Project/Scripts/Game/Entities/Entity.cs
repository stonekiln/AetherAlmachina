using R3;
using UnityEngine;
using System.Linq;
using DConfig.StageLife.Event;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using DIVFactor.Injectable;
using AetherAlmachina.Deck;
using AetherAlmachina.Entities.Parameter;
using DIVFactor.Extensions;
using System;
using Tools.Helpers;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// エンティティのMonoBehaviour
    /// </summary>
    public abstract class Entity : MonoBehaviour, IEntityInteraction, IInjectable
    {
        public string Name { get; private set; }
        public LockOnEventBundle LockOn { get; private set; }
        public InteractionEventBundle Interaction { get; private set; }
        public EventBus<LayoutIndexEvent> LayoutIndexSet { get; private set; }
        public Vector2Int LayoutIndex { get; private set; }
        public StatusParameter Status { get; private set; }
        protected EventBus<AutoIncreaseEvent> AutoIncrease;
        protected EventBus<DeckGetEvent> DeckGet;
        protected DeckListAsset deckList;

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out LockOnEventBundle lockOn);
            resolver.Inject(out InteractionEventBundle interaction);
            resolver.Inject(out StatusAsset statusAsset);
            resolver.Inject(out DeckController deckController);
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out EventBus<LayoutIndexEvent> layoutIndexSet);

            Name = gameObject.name;
            LockOn = lockOn;
            Interaction = interaction;
            Status = new(statusAsset, new(Interaction.ResourceUpdate, this), Interaction.ResourceUpdate.HP.Request);
            deckList = statusAsset.Deck;
            LayoutIndexSet = layoutIndexSet;

            deckController.Subscribe(this);
            AutoIncrease.Switch(Interaction.ResourceUpdate.Cost.Request).Subscribe(log => new(log.Delta)).AddTo(this);
            LayoutIndexSet.Subscribe(log => LayoutIndex = log.Index);

            Interaction.ResourceUpdate.HP.Response.Where(log => log.Current <= 0).Take(1).Subscribe(_ =>
            {
                Debug.Log(gameObject.name + "が死亡しました。");
                resolver.EntryEndPoint();
            }).AddTo(this);

            resolver.ActivePoint.Subscribe(_ => Get());
        }

        void Get()
        {
            Debug.Log("デッキをセットしました");
            DeckGet.OnNext(new(deckList.ReadDeck(this).ToList()));
        }
    }
}
