using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : MonoBehaviour
{
    const int HandLimit = 5;
    [SerializeField] GameObject playerObject;
    public List<SkillData> deck;
    List<GameObject> hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck.ForEach(card => card.SetOwner.Invoke(playerObject));
        hand = new List<GameObject>();
        Draw(HandLimit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Draw(int count)
    {
        GameObject WrappedSetParent(GameObject child)
        {
            child.transform.SetParent(transform, false);
            return child;
        }

        hand = hand.Concat(deck.Take(count).Select(card => WrappedSetParent(card.Create.Invoke())))
                   .OrderBy(card => card.transform.GetChild(0).GetComponent<CardData>().Cost)
                   .Select((card, index) =>
                   {
                       card.transform.SetSiblingIndex(index);
                       return card;
                   }).ToList();

        deck = deck.GetRange(count, deck.Count - count);
    }
}
