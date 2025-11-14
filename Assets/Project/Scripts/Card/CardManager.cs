using System;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject CardObject { get; private set; }
    public GameObject HitBox { get; private set; }
    public CardDesign Design { get; private set; }
    public CardSelecter Selecter { get; private set; }
    public int Cost { get; private set; }
    public HandController callBacks;

    void Awake()
    {
        CardObject = transform.GetChild(0).gameObject;
        HitBox = transform.GetChild(1).gameObject;
        Design = CardObject.GetComponent<CardDesign>();
        Selecter = HitBox.GetComponent<CardSelecter>();
    }

    public void Initialize(int cardCost, Sprite icon, Action callBack)
    {
        Cost = cardCost;
        Design.Initialize(this, icon);
        Selecter.Initialize(this, callBack);
    }
}