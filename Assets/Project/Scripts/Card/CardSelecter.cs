using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelecter : MonoBehaviour, IButtonBase
{
    public Action onClickCallback;
    public ClickCallBacks callBacks;
    public bool isHover;
    public bool isSelect;
    GameObject designObject;
    readonly Vector2 Offset = new(0, 20f);
    GameObject parentObject;
    Vector2 initialSize;
    readonly Vector2 extraSpacing = new(40f, 0);

    public void Awake()
    {
        isHover = false;
        isSelect = false;
        designObject = transform.parent.GetChild(0).gameObject;
        parentObject = transform.parent.gameObject;
        initialSize = parentObject.GetComponent<RectTransform>().rect.size;
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
        designObject.GetComponent<RectTransform>().anchoredPosition = Offset;
        RectTransform parentRect = parentObject.GetComponent<RectTransform>();
        parentRect.sizeDelta = initialSize + extraSpacing;
    }

    void UnHover()
    {
        isHover = false;
        isSelect = false;
        designObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        RectTransform parentRect = parentObject.GetComponent<RectTransform>();
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
            isSelect = true;
            callBacks.AddCallBacks(onClickCallback);
            callBacks.AddCallBacks(() => UnHover());
        }
        else
        {
            callBacks.Invoke();
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
