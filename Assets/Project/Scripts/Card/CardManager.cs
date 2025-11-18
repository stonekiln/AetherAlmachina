using System;
using EventBus.Card;
using R3;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] CardActiveEvent cardActive;
    public CardDesign Design { get; private set; }
    public CardSelecter Selecter { get; private set; }
    public int Cost { get; private set; }
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    void Awake()
    {
        Design = transform.GetChild(0).GetComponent<CardDesign>();
        Selecter = transform.GetChild(1).GetComponent<CardSelecter>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
        Selecter.isSelect.Where(flag=>flag).Subscribe(_=>cardActive.Add.OnNext((Selecter.onClickCallback+(()=>Destroy(gameObject)),Cost)));
    }

    public void Initialize(int cardCost, Sprite icon, Action callBack)
    {
        Cost = cardCost;
        Selecter.Initialize(this, callBack);
        Design.Initialize(this, icon);
    }

    public void Invoke()
    {
        cardActive.Event.OnNext(true);
    }
}