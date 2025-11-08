using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class CardController : MonoBehaviour
{
    string cardSpritePath = Path.Combine("Sprite","Suite");
    const int handLimit = 5;
    static readonly ReadOnlyCollection<string> SuitKindPath = Array.AsReadOnly(new string[]{
        "Spade", "Club","Diamond","Heart"});

    [NonSerialized] public List<int> playerDeck;
    [NonSerialized] public List<int> enemyDeck;
    public GameObject playerHand;
    public GameObject EnemyHand;
    Sprite[][] suitSprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CardMaker cardMaker = new();
        playerDeck = cardMaker.PlayerDeck;
        enemyDeck = cardMaker.EnemyDeck;

        suitSprites = new Sprite[SuitKindPath.Count][];
        SuitKindPath.Select((path, index) => suitSprites[index] = Resources.LoadAll<Sprite>(Path.Combine(cardSpritePath, path)));

        Show.Array(playerDeck);
    }
}

public class CardMaker
{
    const int DeckNumber = 52;
    public List<int> PlayerDeck { get; private set; }
    public List<int> EnemyDeck { get; private set; }

    public CardMaker()
    {
        List<int> deck = Enumerable.Range(0,DeckNumber).ToList();

        PlayerDeck = FisherYates(deck);
        EnemyDeck = FisherYates(deck);
    }

    List<int> FisherYates(List<int> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int RandomNumber = UnityEngine.Random.Range(0, i);
            (array[i], array[RandomNumber]) = (array[RandomNumber], array[i]);
        }
        return array;
    }
}