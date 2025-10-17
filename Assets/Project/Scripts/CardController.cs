using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class CardController : MonoBehaviour
{
    const string CardSpritePath = "Sprite/Suite/";
    const int handLimit = 5;
    static readonly ReadOnlyCollection<string> SuitKindPath = Array.AsReadOnly(new string[]{
        "Spade", "Club","Diamond","Heart","Other" });

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
        SuitKindPath.Select((path, index) => suitSprites[index] = Resources.LoadAll<Sprite>(CardSpritePath + path));

    }
}

public class CardMaker
{
    const int DeckNumber = 27;
    public List<int> PlayerDeck { get; private set; }
    public List<int> EnemyDeck { get; private set; }

    public CardMaker()
    {
        //14はjoker
        List<int> deck = Enumerable.Range(0,DeckNumber).Select((i)=>(int)Mathf.Ceil((i + 1) / 2)).ToList();

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