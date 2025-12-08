using LSES;
using LSES.Battle.Event;
using LSES.EntryPoint;
using UnityEngine;
using VContainer;

public class Enemy : Entity
{
    [SerializeField] GameObject playerObject;

    [Inject]
    public void Construct(EventBus<AutoIncreaseEvent> autoIncrease, EventBus<DeckGetEvent> deckGet,EventBus<PreStartEvent> preStart)
    {
        PreStart=preStart;
        AutoIncrease = autoIncrease;
        DeckGet = deckGet;
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
