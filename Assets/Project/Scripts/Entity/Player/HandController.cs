using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class HandController : MonoBehaviour
{
    const int HandLimit = 5;
    public List<SkillData> deck;
    List<GameObject> hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = new List<GameObject>();
        Draw(HandLimit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Draw(int count)
    {
        hand.AddRange(deck.Take(count).Select(card => Instantiate(card.CardObject, transform)));
        deck = deck.GetRange(count, deck.Count - count);
    }

    void HandAdjust()
    {
        
    }
}
