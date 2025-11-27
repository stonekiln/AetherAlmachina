using LSES.Battle.Event;
using UnityEngine;
using VContainer;

public class DeckManager : MonoBehaviour
{
    DeckEventBundle PlayerDeck;
    DeckEventBundle EnemyDeck;

    [Inject]
    void Construct(DeckEventBundle deckEvents)
    {
        PlayerDeck = deckEvents;
        EnemyDeck = deckEvents;
    }

    void OnEnable()
    {
        PlayerDeck.Subscribe(this);
        EnemyDeck.Subscribe(this);
    }
}