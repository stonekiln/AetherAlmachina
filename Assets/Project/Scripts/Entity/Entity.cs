using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected float hitPoint;
    protected float magicPoint;
    protected float attack;
    protected float defence;
    protected float power;
    protected float handPower;
    [NonSerialized] public Entity target;

    public void Attack(float skillPower)
    {
        target.Hit(attack, power * handPower * skillPower);
    }
    public virtual void Hit(float attackerAttack, float skillPower)
    {
        hitPoint += ((attackerAttack - defence > 0) ? attackerAttack - defence : 0) * skillPower;
    }
}
