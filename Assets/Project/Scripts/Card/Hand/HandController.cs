using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : HandExecutor
{
    const int HandLimit = 5;
    [SerializeField] HandPowerTable handPowerTable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck.ForEach(card => handMonitoringEntity.SetOwnerEvent.OnNext(card));
        hand = new List<CardManager>();
        Draw(HandLimit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Draw(int count)
    {
        CardManager SetCard(CardManager child)
        {
            child.transform.SetParent(transform, false);
            return child;
        }
        CardManager SetHand(CardManager child, int index)
        {
            child.transform.SetSiblingIndex(index);
            return child;
        }

        hand = hand.Where(card => card != null)
                   .Concat(deck.Take(count).Select(card => SetCard(card.CreateObject().GetComponent<CardManager>())))
                   .OrderBy(card => card.Data.Cost).Select((card, index) => SetHand(card, index)).ToList();
        deck = deck.GetRange(count, deck.Count - count);
    }

    protected override void SetHandPower(int type, int count)
    {
        handMonitoringEntity.SetHandPowerEvent.OnNext(handPowerTable.Get(type, count));
    }
}
