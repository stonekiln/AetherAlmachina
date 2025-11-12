using UnityEngine;

public class Player : Entity
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Hit(float attackerAttack, float skillPower)
    {
        base.Hit(attackerAttack, skillPower);
        Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + hitPoint);
    }
}
