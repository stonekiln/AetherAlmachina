using System;
using DivFacter.Event;
using DConfig.BattleLife.Event;
using DConfig.PalyerLife.Event;
using DivFacter.EntryPoint;
using R3;
using UnityEngine;
using System.Linq;

/// <summary>
/// エンティティのMonoBehaviour
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected EventBus<AutoIncreaseEvent> AutoIncrease;
    protected EventBus<DeckGetEvent> DeckGet;
    protected EventBus<PreStartEvent> PreStart;
    [SerializeField] protected StatusAsset statusAsset;
    protected DeckController deckController;
    public Status Status { get; private set; }
    protected float power;
    protected float handPower;
    [NonSerialized] public Entity target;

    void Awake()
    {
        Status = new(statusAsset);
        power = 1;
        handPower = 1;
    }

    void OnEnable()
    {
        PreStart.Subscribe(_ => Get()).AddTo(this);
        AutoIncrease.Subscribe(log => CostIncrease(log.Delta)).AddTo(this);
        deckController.Subscribe(this);
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
