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
}
