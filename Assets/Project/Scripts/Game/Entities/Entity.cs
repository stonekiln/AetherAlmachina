using R3;
using UnityEngine;
using System.Linq;
using DConfig.StageLife.Event;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using DIVFactor.Injectable;
using AetherAlmachina.Deck;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// エンティティのMonoBehaviour
    /// </summary>
    public abstract class Entity : MonoBehaviour, ICombatInteraction, IInjectable
    {
        protected EventBus<AutoIncreaseEvent> AutoIncrease;
        protected EventBus<DeckGetEvent> DeckGet;
        protected EventBus<SkillActiveEvent> SkillActive;
        protected TargetingEventBundle targeting;
        protected DeckList deckList;
        protected DeckController deckController;
        protected float power;
        protected float handPower;
        public Status Status { get; private set; }
        public TargetingEventBundle Targeting => targeting;
        public int SiblingIndex => transform.GetSiblingIndex();

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out StatusAsset statusAsset);
            Status = new(statusAsset);
            deckList = statusAsset.Deck;
            power = 1;
            handPower = 1;
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out deckController);
            resolver.Inject(out SkillActive);
            resolver.Inject(out targeting);

            AutoIncrease.Subscribe(log => CostIncrease(log.Delta)).AddTo(this);
            deckController.Subscribe(this);
            SkillActive.Subscribe(log =>
            {
                Debug.Log(log.Data.Name + "が発動しました。");
                while (log.Data.MoveNext()) ;
            }).AddTo(this);
            Targeting.Hit.Subscribe(log => log.Apply(this)).AddTo(this);

            resolver.ActivePointAsObservable().Subscribe(_ => Get());
        }

        public void Attack(Entity target, float skillPower)
        {
            target.Hit(Status.Attack, power * handPower * skillPower);
        }
        public void Hit(int attackerAttack, float power)
        {
            Status.hitPoint += ((Status.Defence - attackerAttack < 0) ? Status.Defence - attackerAttack : 0) * power;
            Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + Status.hitPoint);
        }
        public void Get()
        {
            Debug.Log("デッキをセットしました");
            DeckGet.OnNext(new(deckList.ReadDeck(this).ToList()));
        }
        public void SetHandPower(float power)
        {
            handPower = power;
        }
        void CostIncrease(int delta)
        {
            Status.MPFluctuation.OnNext(new());
            Status.magicPoint += delta;
        }
    }
}