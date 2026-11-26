using AetherAlmachina.Deck;
using AetherAlmachina.Entities.Parameter;
using DConfig.EntityLife.Event;
using DConfig.StageLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using R3;
using System.Linq;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// エンティティのMonoBehaviour
    /// </summary>
    public abstract class Entity : MonoBehaviour, IEntityInteraction, IActiveComponent
    {
        public string Name { get; private set; }
        public LockOnEventHub LockOn { get; private set; }
        public InteractionEventHub Interaction { get; private set; }
        public EventPort<LayoutIndexEvent> LayoutIndexSet { get; private set; }
        public Vector2Int LayoutIndex { get; private set; }
        public StatusParameter Status { get; private set; }
        protected EventPort<AutoIncreaseEvent> AutoIncrease;
        protected EventPort<DeckGetEvent> DeckGet;
        protected DeckListAsset deckList;

        StatusAsset statusAsset;
        DeckController deckController;
        LifecycleEventHub lifecycle;

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out LockOnEventHub lockOn);
            resolver.Inject(out InteractionEventHub interaction);
            resolver.Inject(out statusAsset);
            resolver.Inject(out deckController);
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out EventPort<LayoutIndexEvent> layoutIndexSet);

            resolver.Inject(out lifecycle);
            LockOn = lockOn;
            Interaction = interaction;
            LayoutIndexSet = layoutIndexSet;
            resolver.Injected.SubscribeLifecycle(Initialize, resolver.Fail);
        }

        public virtual void Initialize()
        {
            Name = gameObject.name;
            Status = new(statusAsset, new ResourceStatusParameter());
            deckList = statusAsset.Deck;
        }

        public virtual void Subscribe(SubscriptionContext context)
        {
            Status.Resource.Subscribe(Interaction.ResourceUpdate, context.Lifetime);
            deckController.Subscribe(context.Lifetime);
            AutoIncrease.Switch(Interaction.ResourceUpdate.Cost.Request).Subscribe(log => new(log.Delta)).AddTo(context.Lifetime);
            LayoutIndexSet.Subscribe(log => LayoutIndex = log.Index).AddTo(context.Lifetime);

            Interaction.ResourceUpdate.HP.Response.Where(log => log.Current <= 0).Take(1).Subscribe(_ =>
            {
                Debug.Log(gameObject.name + "が死亡しました。");
                lifecycle.Release.OnNext(new());
            }).AddTo(context.Lifetime);
            context.Subscribed.SubscribeLifecycle(Activate, context.Fail);
        }

        public virtual void Activate()
        {
            // 初期通知は、同じLifetime内の全購読が揃ってから発行する。
            Interaction.ResourceUpdate.HP.Request.OnNext(new(Status.GetInt(StatusType.MaxHitPoint)));
            Get();
        }

        void Get()
        {
            Debug.Log("デッキをセットしました");
            DeckGet.OnNext(new(deckList.ReadDeck(this).ToList()));
        }
    }
}
