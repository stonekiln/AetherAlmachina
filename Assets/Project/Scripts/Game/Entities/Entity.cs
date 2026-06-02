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
    public abstract class Entity : MonoBehaviour, IEntityEnchantInteraction, IInjectable
    {
        public TargetingEventBundle Targeting { get; private set; }
        public ActionEventBundle Action { get; private set; }
        public ProcessEventBundle Process { get; private set; }
        public EventBus<LayoutIndexEvent> LayoutIndexSet { get; private set; }
        public Vector2Int LayoutIndex { get; private set; }
        public StatusParameter Status { get; private set; }
        protected EventBus<AutoIncreaseEvent> AutoIncrease;
        protected EventBus<DeckGetEvent> DeckGet;
        protected DeckListAsset deckList;
        IStatusReader IEntityInteraction.Status => Status;
        IEnchantableStatus IEntityEnchantInteraction.Status => Status;

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out TargetingEventBundle targeting);
            resolver.Inject(out ActionEventBundle action);
            resolver.Inject(out ProcessEventBundle process);
            resolver.Inject(out StatusAsset statusAsset);
            resolver.Inject(out DeckController deckController);
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out EventBus<LayoutIndexEvent> layoutIndexSet);

            Targeting = targeting;
            Action = action;
            Process = process;
            Status = new(statusAsset, new(Process, this), Process.ResourceUpdate.HP.Request);
            deckList = statusAsset.Deck;
            LayoutIndexSet = layoutIndexSet;

            deckController.Subscribe(this);
            AutoIncrease.Switch(Process.ResourceUpdate.Cost.Request).Subscribe(log => new(log.Delta)).AddTo(this);
            LayoutIndexSet.Subscribe(log => LayoutIndex = log.Index);

            Targeting.Hit.Subscribe(log => log.Apply(this)).AddTo(this);

            Action.Attack.Subscribe(log => Attack(log.Target, log.SkillPower)).AddTo(this);
            Action.Damage.Subscribe(log => Damage(log.Attack, log.Power));
            Action.Heal.Subscribe(log => Heal(log.Target, log.SkillPower)).AddTo(this);
            Action.Recovery.Subscribe(log => Recovery(log.Recovery, log.Power)).AddTo(this);

            Process.ResourceUpdate.HP.Response.Where(log => log.Current <= 0).Take(1).Subscribe(_ =>
            {
                Debug.Log(gameObject.name + "が死亡しました。");
                resolver.EntryEndPoint();
            }).AddTo(this);

            resolver.ActivePoint.Subscribe(_ => Get());
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
                Process.ResourceUpdate.Disable.Request.OnNext(new(-1));
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
                        Process.ResourceUpdate.Shield.Request.OnNext(new(shieldDamage));
                        Debug.Log(gameObject.name + "が" + -shieldDamage + "のシールドを消費しました\n残りシールド:" + Status.Resource.Shield);
                    }
                    Process.ResourceUpdate.HP.Request.OnNext(new(remainingShield));
                    Debug.Log(gameObject.name + "が" + -remainingShield + "ダメージを受けました。\n残りHP:" + Status.Resource.HitPoint);
                }
                else
                {
                    Process.ResourceUpdate.Shield.Request.OnNext(new(damageAmount));
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
            Process.ResourceUpdate.HP.Request.OnNext(new(healAmount));
            Debug.Log(gameObject.name + "のHPが" + healAmount + "回復しました。\n残りHP:" + Status.Resource.HitPoint);
        }
        void Get()
        {
            Debug.Log("デッキをセットしました");
            DeckGet.OnNext(new(deckList.ReadDeck(this).ToList()));
        }
    }
}
