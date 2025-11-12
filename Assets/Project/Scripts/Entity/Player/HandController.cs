using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        hand.AddRange(deck.Take(count).Select(card => SetPareantGetChild(card.Create.Invoke())));
        deck = deck.GetRange(count, deck.Count - count);
    }

    GameObject SetPareantGetChild(GameObject child)
    {
        child.transform.SetParent(transform, false);
        return child;
    }

    void HandAdjust()
    {

    }
}
