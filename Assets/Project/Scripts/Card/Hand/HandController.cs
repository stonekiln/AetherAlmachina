using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : MonoBehaviour
{
    const int HandLimit = 5;
    [SerializeField] GameObject playerObject;
    public List<SkillData> deck;
    List<GameObject> hand;
    ClickCallBacks clickCallBacks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck.ForEach(card => card.SetOwner.Invoke(playerObject));
        clickCallBacks = new();
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
            child.transform.GetChild(1).GetComponent<CardSelecter>().callBacks = clickCallBacks;
            return child;
        }
        GameObject WrappedSetIndex(GameObject child, int index)
        {
            child.transform.SetSiblingIndex(index);
            return child;
        }

        hand = hand.Concat(deck.Take(count).Select(card => WrappedSetParent(card.Create.Invoke())))
                   .OrderBy(card => card.transform.GetChild(0).GetComponent<CardData>().Cost)
                   .Select((card, index) => WrappedSetIndex(card, index)).ToList();

        deck = deck.GetRange(count, deck.Count - count);
    }
}
