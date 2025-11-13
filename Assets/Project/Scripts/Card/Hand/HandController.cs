using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : MonoBehaviour
{
    const int HandLimit = 5;
    [SerializeField] GameObject playerObject;
    [SerializeField] HandPowerTable handPowerTable;
    public List<SkillData> deck;
    List<GameObject> hand;
    ClickCallBacks clickCallBacks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck.ForEach(card => card.SetOwner.Invoke(playerObject));
        clickCallBacks = new((type,count)=>SetHandPower(type,count));
        hand = new List<GameObject>();
        Draw(HandLimit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Draw(int count)
    {
        GameObject SetCard(GameObject child)
        {
            child.transform.SetParent(transform, false);
            child.GetComponent<CardManager>().callBacks = clickCallBacks;
            return child;
        }
        GameObject SetHand(GameObject child, int index)
        {
            child.transform.SetSiblingIndex(index);
            return child;
        }

        hand = hand.Concat(deck.Take(count).Select(card => SetCard(card.Create.Invoke())))
                   .OrderBy(card => card.GetComponent<CardManager>().Cost)
                   .Select((card, index) => SetHand(card, index)).ToList();

        deck = deck.GetRange(count, deck.Count - count);
    }

    void SetHandPower(int type,int count)
    {
        //Debug.Log(handPowerTable.Get(type, count));
        playerObject.GetComponent<Entity>().SetHandPower(handPowerTable.Get(type,count));        
    }
}
