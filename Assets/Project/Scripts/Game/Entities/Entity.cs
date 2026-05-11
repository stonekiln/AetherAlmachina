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
        public Vector2Int LayoutIndex { get; private set; }
        Action entryEnd;

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out StatusAsset statusAsset);
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out deckController);
            resolver.Inject(out targeting);
            resolver.Inject(out action);
            resolver.Inject(out process);

            Status = new(statusAsset, new(Process, this), Process.ResourceUpdate.HP);
            deckList = statusAsset.Deck;

            AutoIncrease.Switch(Process.CostUpdate).Subscribe(log => new(log.Delta)).AddTo(this);
            deckController.Subscribe(this);
            Targeting.Hit.Subscribe(log => log.Apply(this)).AddTo(this);

            Action.Attack.Subscribe(log => Attack(log.Target, log.SkillPower)).AddTo(this);
            Action.Damage.Subscribe(log => Damage(log.Attack, log.Power));
            Action.Heal.Subscribe(log => Heal(log.Target, log.SkillPower)).AddTo(this);
            Action.Recovery.Subscribe(log => Recovery(log.Recovery, log.Power)).AddTo(this);

            Process.EntityDeath.Subscribe(_ => DeathCheck());

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
            int damageAmount = Mathf.FloorToInt(damage);
            if (Status.Resource.Disable > 0)
            {
                Process.ResourceUpdate.Disable.OnNext(new(-1));
                Debug.Log(gameObject.name + "がダメージを無効化しました。\n残り回数:" + Status.Resource.Disable);
            }
            else
            {
                int remainingShield = Status.Resource.Shield + damageAmount;
                if (remainingShield < 0)
                {
                    if (Status.Resource.Shield > 0)
                    {
                        int shieldDamage = -Status.Resource.Shield;
                        Process.ResourceUpdate.Shield.OnNext(new(shieldDamage));
                        Debug.Log(gameObject.name + "が" + -shieldDamage + "のシールドを消費しました\n残りシールド:" + Status.Resource.Shield);
                    }
                    Process.ResourceUpdate.HP.OnNext(new(remainingShield));
                    Debug.Log(gameObject.name + "が" + -remainingShield + "ダメージを受けました。\n残りHP:" + Status.Resource.HitPoint);
                }
                else
                {
                    Process.ResourceUpdate.Shield.OnNext(new(damageAmount));
                    Debug.Log(gameObject.name + "が" + -damageAmount + "のシールドを消費しました\n残りシールド:" + Status.Resource.Shield);
                }
            }
        }
        void Heal(IEntityInteraction target, float skillPower)
        {
            target.Action.Recovery.OnNext(new(Status.GetInt(StatusType.MaxHitPoint), Status.Get(StatusType.HealPower) * skillPower));
        }
        void Recovery(int recovery, float power)
        {
            int healAmount = Mathf.FloorToInt(Convert.ToSingle(recovery) * Status.Get(StatusType.HealingReceived) * power);
            if ((Status.Resource.HitPoint + healAmount) >= Status.GetInt(StatusType.MaxHitPoint))
            {
                healAmount = Status.GetInt(StatusType.MaxHitPoint) - Status.Resource.HitPoint;
            }
            Process.ResourceUpdate.HP.OnNext(new(healAmount));
            Debug.Log(gameObject.name + "のHPが" + healAmount + "回復しました。\n残りHP:" + Status.Resource.HitPoint);
        }

        void Get()
        {
            Debug.Log("デッキをセットしました");
            DeckGet.OnNext(new(deckList.ReadDeck(this).ToList()));
        }

        public void SetLayoutIndex(Vector2Int layoutIndex)
        {
            LayoutIndex = layoutIndex;
        }
        void DeathCheck()
        {
            if (Status.Resource.HitPoint <= 0)
            {
                Debug.Log(gameObject.name + "が死亡しました。");
                entryEnd();
            }
        }
    }
}
