using System;
using R3;
using UnityEngine;
using System.Linq;
using DConfig.StageLife.Event;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using DIVFactor.Injectable;

/// <summary>
/// エンティティのMonoBehaviour
/// </summary>
public abstract class Entity : MonoBehaviour, IInjectable
{
    protected EventBus<AutoIncreaseEvent> AutoIncrease;
    protected EventBus<DeckGetEvent> DeckGet;
    protected StatusAsset statusAsset;
    protected DeckController deckController;
    public Status Status { get; private set; }
    protected float power;
    protected float handPower;
    [NonSerialized] public Entity target;

    public virtual void Injection(InjectableResolver resolver)
    {
        resolver.Inject(out statusAsset);
        Status = new(statusAsset);
        power = 1;
        handPower = 1;
        resolver.Inject(out AutoIncrease);
        resolver.Inject(out DeckGet);
        resolver.Inject(out deckController);

        AutoIncrease.Subscribe(log => CostIncrease(log.Delta)).AddTo(this);
        deckController.Subscribe(this);
        resolver.GetComponent<EventBus<ActivateEvent>>().Subscribe(_ => Get());
    }

    public void Set(StatusAsset asset)
    {
        statusAsset = asset;
    }

    public void Attack(float skillPower)
    {
        target.Hit(Status.attack, power * handPower * skillPower);
    }
    public void Hit(float attackerAttack, float power)
    {
        Status.hitPoint += ((Status.defence - attackerAttack < 0) ? Status.defence - attackerAttack : 0) * power;
        Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + Status.hitPoint);
    }
    public void Get()
    {
        Debug.Log("デッキをセットしました");
        DeckGet.Publish(new(statusAsset.Deck.ReadDeck().Select(card => card.SetOwner(this)).ToList()));
    }
    public void SetHandPower(float power)
    {
        handPower = power;
    }
    void CostIncrease(int delta)
    {
        Status.MPfluctuation.Publish(new());
        Status.magicPoint += delta;
    }
}
