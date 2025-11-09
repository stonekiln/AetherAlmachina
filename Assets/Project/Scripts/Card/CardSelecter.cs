using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelecter : MonoBehaviour, ButtonBase
{
    public System.Action onClickCallback;
    Vector3 baseScale;
    bool isHover;

    public void Awake()
    {
        baseScale = transform.localScale;
        isHover = false;
    }

    public void SetActive()
    {
        transform.localScale = baseScale;
    }

    public void SetInActive()
    {
        transform.localScale = Vector3.zero;
    }

    void Hover()
    {
        isHover = true;
    }

    void DeHover()
    {
        isHover = false;
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
