using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelecter : MonoBehaviour, IButtonBase
{
    public Action onClickCallback;
    CardManager cardData;
    [NonSerialized] public bool isHover;
    [NonSerialized] public bool isSelect;
    readonly Vector2 Offset = new(0, 20f);
    Vector2 initialSize;
    readonly Vector2 extraSpacing = new(40f, 0);

    public void Initialize(CardManager cardManager, Action Callback)
    {
        cardData = cardManager;
        onClickCallback = Callback;
        isHover = false;
        isSelect = false;
        initialSize = cardManager.gameObject.GetComponent<RectTransform>().rect.size;
    }

    public void SetActive()
    {

    }

    public void SetInActive()
    {
        transform.localScale = Vector3.zero;
    }

    void Hover()
    {
        isHover = true;
        cardData.CardObject.GetComponent<RectTransform>().anchoredPosition = Offset;
        RectTransform parentRect = cardData.gameObject.GetComponent<RectTransform>();
        parentRect.sizeDelta = initialSize + extraSpacing;
    }

    void UnHover()
    {
        isHover = false;
        isSelect = false;
        cardData.CardObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        RectTransform parentRect = cardData.gameObject.GetComponent<RectTransform>();
        parentRect.sizeDelta = initialSize;
    }

    void Push()
    {

    }

    void Release()
    {
        if (isHover)
        {

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelect)
        {
            isSelect = cardData.callBacks.AddCallBacks(onClickCallback + (() => Destroy(cardData.gameObject)), cardData.Cost);
        }
        else
        {
            cardData.callBacks.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Push();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelect)
        {
            UnHover();
        }
    }
}
