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
using Utility;
using System;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// エンティティのMonoBehaviour
    /// </summary>
    public abstract class Entity : MonoBehaviour, IEntityInteraction, IInjectable
    {
        TargetingEventBundle targeting;
        ActionEventBundle action;
        ProcessEventBundle process;
        protected EventBus<AutoIncreaseEvent> AutoIncrease;
        protected EventBus<DeckGetEvent> DeckGet;
        protected DeckListAsset deckList;
        protected DeckController deckController;
        public StatusParameter Status { get; private set; }
        public TargetingEventBundle Targeting => targeting;
        public ActionEventBundle Action => action;
        public ProcessEventBundle Process => process;
        public int SiblingIndex => transform.GetSiblingIndex();
        Action entryEnd;

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out StatusAsset statusAsset);
            Status = new(statusAsset);
            deckList = statusAsset.Deck;
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out deckController);
            resolver.Inject(out targeting);
            resolver.Inject(out action);
            resolver.Inject(out process);

            AutoIncrease.Switch(process.MPUpdate).Subscribe(log => new(log.Delta)).AddTo(this);
            deckController.Subscribe(this);
            Targeting.Hit.Subscribe(log => log.Apply(this)).AddTo(this);
            Action.Attack.Subscribe(log => Attack(log.Target, log.SkillPower)).AddTo(this);
            Action.Damage.Subscribe(log => Damage(log.Attack, log.Power)).AddTo(this);
            Action.Healing.Subscribe(log => Heal(log.Target, log.SkillPower)).AddTo(this);
            Action.OnHealed.Subscribe(log => Healing(log.Recovery, log.Power)).AddTo(this);
            Process.MPUpdate.Subscribe(log => Status.MPUpdate(log.Delta)).AddTo(this);

            resolver.ActivePoint.Subscribe(_ => Get());
            entryEnd = resolver.EntryEndPoint;
        }

        void Attack(IEntityInteraction target, float skillPower)
        {
            float attackerPower = Status.Get(StatusType.Power);
            if (Probability.Try(Status.Get(StatusType.CriticalRate)))
            {
                Debug.Log("クリティカルが発生しました。");
                attackerPower *= Status.Get(StatusType.CriticalDamage);
            }
            target.Action.Damage.OnNext(new(Status.GetInt(StatusType.Attack), attackerPower * skillPower));
        }
        void Damage(int attack, float power)
        {
            float damage = Convert.ToSingle(Status.GetInt(StatusType.Defence) - attack);
            if (damage > 0f) damage = 0f;
            damage *= Status.Get(StatusType.DamageTaken) * power;

            Status.hitPoint += damage;
            if (Status.hitPoint > 0)
            {
                Debug.Log(gameObject.name + "が" + -damage + "ダメージを受けました。\n残りHP:" + Status.hitPoint);
            }
            else
            {
                Debug.Log(gameObject.name + "が死亡しました。");
                entryEnd();
            }
        }
        void Heal(IEntityInteraction target, float skillPower)
        {
            target.Action.OnHealed.OnNext(new(Status.GetInt(StatusType.MaxHitPoint), Status.Get(StatusType.HealPower) * skillPower));
        }
        void Healing(int recovery, float power)
        {
            float healAmount = Convert.ToSingle(recovery) * Status.Get(StatusType.HealingReceived) * power;
            Status.hitPoint += healAmount;
            if (Status.hitPoint > Status.Get(StatusType.MaxHitPoint))
            {
                Status.hitPoint = Status.Get(StatusType.MaxHitPoint);
            }
            Debug.Log(gameObject.name + "のHPが" + healAmount + "回復しました。\n残りHP:" + Status.hitPoint);
        }
        void Get()
        {
            Debug.Log("デッキをセットしました");
            DeckGet.OnNext(new(deckList.ReadDeck(this).ToList()));
        }
    }
}