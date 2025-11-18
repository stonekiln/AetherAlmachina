using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : HandExecutor
{
    const int HandLimit = 5;
    [SerializeField] GameObject playerObject;
    [SerializeField] HandPowerTable handPowerTable;
    public List<SkillData> deck;
    List<GameObject> hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        deck.ForEach(card => card.SetOwner.Invoke(playerObject));
        hand = new List<GameObject>();
        Draw(HandLimit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Draw(int count)
    {
        GameObject SetCard(GameObject child)
        {
            child.transform.SetParent(transform, false);
            return child;
        }
        GameObject SetHand(GameObject child, int index)
        {
            child.transform.SetSiblingIndex(index);
            return child;
        }

        hand = hand.Where(card => card != null)
                   .Concat(deck.Take(count).Select(card => SetCard(card.Create.Invoke())))
                   .OrderBy(card => card.GetComponent<CardManager>().Cost)
                   .Select((card, index) => SetHand(card, index)).ToList();

        deck = deck.GetRange(count, deck.Count - count);
    }

    protected override void SetHandPower(int type, int count)
    {
        playerObject.GetComponent<Entity>().SetHandPower(handPowerTable.Get(type, count));
    }
}
