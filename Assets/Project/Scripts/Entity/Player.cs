using LSES;
using LSES.Battle.Event;
using LSES.EntryPoint;
using VContainer;

public class Player : Entity
{
    [Inject]
    public void Construct(EventBus<AutoIncreaseEvent> autoIncrease, EventBus<DeckGetEvent> playerDeckGet,EventBus<PreStartEvent> preStart)
    {
        PreStart=preStart;
        AutoIncrease = autoIncrease;
        DeckGet = playerDeckGet;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
