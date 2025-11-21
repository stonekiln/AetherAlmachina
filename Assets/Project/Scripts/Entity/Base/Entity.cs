using System;
using EventBus.Cost;
using R3;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected StatusAsset statusAsset;
    [SerializeField] protected AutoIncreaseEvent AutoIncreaseEvent;
    protected Status status;
    protected float power;
    protected float handPower;
    [NonSerialized] public Entity target;

    void Awake()
    {
        status = new(statusAsset);
        power = 1;
        handPower = 1;
        AutoIncreaseEvent.Channel.Subscribe(delta => CostIncrease(delta)).AddTo(this);
        statusAsset.SetOwnerEvent.Subscribe(cardData=>cardData.SetOwner(this)).AddTo(this);
        statusAsset.SetHandPowerEvent.Subscribe(power => SetHandPower(power)).AddTo(this);
    }

    public void Attack(float skillPower)
    {
        target.Hit(status.attack, power * handPower * skillPower);
    }
    public void Hit(float attackerAttack, float power)
    {
        status.hitPoint += ((status.defence - attackerAttack < 0) ? status.defence - attackerAttack : 0) * power;
        Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + status.hitPoint);
    }
    public void SetHandPower(float power)
    {
        handPower = power;
    }
    void CostIncrease(int delta)
    {
        statusAsset.magicPoint.Value += delta;
    }
}
