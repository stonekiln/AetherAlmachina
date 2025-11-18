using System;
using DG.Tweening;
using R3;
using UnityEngine;

public class CardSelecter : ButtonBase
{
    CardManager cardData;
    readonly Vector2 ExtraSpacing = new(40f, 0);
    readonly Vector2 Offset = new(0, 20f);

    void Awake()
    {
        OnPointerClickAsObservable().Subscribe(eventData => MyPointerClick()).AddTo(this);
        OnPointerDownAsObservable().Subscribe(eventData => Push()).AddTo(this);
        OnPointerUpAsObservable().Subscribe(eventData => Release()).AddTo(this);
        OnPointerEnterAsObservable().Subscribe(eventData => Hover()).AddTo(this);
        OnPointerExitAsObservable().Subscribe(eventData => UnHover()).AddTo(this);
    }

    public void Initialize(CardManager cardManager, Action Callback)
    {
        cardData = cardManager;
        onClickCallback = Callback;
    }

    public override void SetActive()
    {

    }

    public override void SetInActive()
    {
        transform.localScale = Vector3.zero;
    }

    protected override void Hover()
    {
        isHover.Value = true;
        cardData.rectTransform.sizeDelta = cardData.initialSize + ExtraSpacing;
        cardData.Design.rectTransform.anchoredPosition = cardData.Design.initialPosition + Offset;
    }

    protected override void UnHover()
    {
        if (!isSelect.Value)
        {
            isHover.Value = false;
            cardData.rectTransform.sizeDelta = cardData.initialSize;
            cardData.Design.rectTransform.anchoredPosition = cardData.Design.initialPosition;
        }
    }

    protected override void Push()
    {

    }

    protected override void Release()
    {
        if (isHover.Value)
        {

        }
    }

    public void MyPointerClick()
    {
        if (!isSelect.Value)
        {
            isSelect.Value = true;
        }
        else
        {
            cardData.Invoke();
        }
    }
}
