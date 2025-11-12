using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] GameObject playerObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = playerObject.GetComponent<Player>();
        Encount();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Encount()
    {
        playerObject.GetComponent<Player>().target = gameObject.GetComponent<Enemy>();
    }
    
    public override void Hit(float attackerAttack, float skillPower)
    {
        base.Hit(attackerAttack, skillPower);
        Debug.Log(gameObject.name + "が攻撃を受けました。\n残りHP:" + hitPoint);
    }
}
