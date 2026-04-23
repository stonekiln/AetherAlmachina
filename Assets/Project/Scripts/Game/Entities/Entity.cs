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
    public abstract class Entity : MonoBehaviour, IEntityInteraction, IInjectable
    {
        TargetingEventBundle targeting;
        CommandEventBundle command;
        protected EventBus<AutoIncreaseEvent> AutoIncrease;
        protected EventBus<DeckGetEvent> DeckGet;
        protected DeckList deckList;
        protected DeckController deckController;
        protected float handPower;
        public StatusParameter Status { get; private set; }
        public TargetingEventBundle Targeting => targeting;
        public CommandEventBundle Command => command;
        public int SiblingIndex => transform.GetSiblingIndex();

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out StatusAsset statusAsset);
            Status = new(statusAsset);
            deckList = statusAsset.Deck;
            handPower = 1;
            resolver.Inject(out AutoIncrease);
            resolver.Inject(out DeckGet);
            resolver.Inject(out deckController);
            resolver.Inject(out targeting);
            resolver.Inject(out command);

            AutoIncrease.Subscribe(log => CostIncrease(log.Delta)).AddTo(this);
            deckController.Subscribe(this);
            Targeting.Hit.Subscribe(log => log.Apply(this)).AddTo(this);
            command.Attack.Subscribe(log => Attack(log.Target, log.SkillPower)).AddTo(this);
            command.Damage.Subscribe(log => Damage(log.Attack, log.Power)).AddTo(this);

            resolver.ActivePoint.Subscribe(_ => Get());
        }

        void Attack(Entity target, float skillPower)
        {
            target.command.Damage.OnNext(new(Status.GetInt(StatusType.Attack), Status.Get(StatusType.Power) * handPower * skillPower));
        }
        void Damage(int attackerAttack, float power)
        {
            Status.hitPoint += ((Status.GetInt(StatusType.Defence) - attackerAttack < 0) ? Status.GetInt(StatusType.Defence) - attackerAttack : 0) * power;
            Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + Status.hitPoint);
        }
        void Get()
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