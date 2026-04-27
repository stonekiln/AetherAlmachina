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
            action.Attack.Subscribe(log => Attack(log.Target, log.SkillPower)).AddTo(this);
            action.Damage.Subscribe(log => Damage(log.Attack, log.Power)).AddTo(this);
            process.MPUpdate.Subscribe(log => Status.MPUpdate(log.Delta)).AddTo(this);

            resolver.ActivePoint.Subscribe(_ => Get());
        }

        void Attack(Entity target, float skillPower)
        {
            target.action.Damage.OnNext(new(Status.GetInt(StatusType.Attack), Status.Get(StatusType.Power) * skillPower));
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
    }
}