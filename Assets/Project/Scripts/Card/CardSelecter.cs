using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelecter : MonoBehaviour, ButtonBase
{
    public Action onClickCallback;
    public bool isHover;
    public GameObject designObject;
    const float OffsetY= 20f;

    public void Awake()
    {
        isHover = false;
        designObject = transform.parent.GetChild(0).gameObject;
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
        designObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, OffsetY);
    }

    void DeHover()
    {
        isHover = false;
        designObject.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, OffsetY);
    }

    void Push()
    {

    }

    void DePush()
    {
        if (isHover)
        {
            
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback.Invoke();
        Destroy(transform.parent.gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Push();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DePush();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DeHover();
    }
}
