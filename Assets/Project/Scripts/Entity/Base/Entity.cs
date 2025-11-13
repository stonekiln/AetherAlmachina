using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected StatusAsset statusAsset;
    protected Status status;
    protected int magicPoint;
    protected float power;
    protected float handPower;
    [NonSerialized] public Entity target;

    void Awake()
    {
        status = new(statusAsset);
        magicPoint = 1;
        power = 1;
        handPower = 1;
    }

    public void Attack(float skillPower)
    {
        target.Hit(status.attack, power * handPower * skillPower);
    }
    public void Hit(float attackerAttack, float skillPower)
    {
        status.hitPoint += ((status.defence - attackerAttack < 0) ? status.defence - attackerAttack : 0) * skillPower;
        Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + status.hitPoint);
    }
}
