using DG.Tweening;
using EventBus.Card;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelecter : ButtonBase
{
    [SerializeField] CardSelectEvent cardSelectEvent;
    CardManager parent;
    RectTransform rectTransform;
    Vector2 initialPosition;
    readonly Vector2 ExtraSpacing = new(40f, 0);
    readonly Vector2 Offset = new(0, 20f);

    void Awake()
    {
        OnPointerClickAsObservable().Subscribe(eventData => MyPointerClick(eventData)).AddTo(this);
        OnPointerDownAsObservable().Subscribe(eventData => Push()).AddTo(this);
        OnPointerUpAsObservable().Subscribe(eventData => Release()).AddTo(this);
        OnPointerEnterAsObservable().Subscribe(eventData => Hover()).AddTo(this);
        OnPointerExitAsObservable().Subscribe(eventData => UnHover()).AddTo(this);
    }

    public void Initialize(CardManager cardManager)
    {
        parent = cardManager;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        onClickCallback = () => cardManager.Data.Activate();
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
        isHover = true;
        parent.rectTransform.sizeDelta = parent.initialSize + ExtraSpacing;
        parent.Design.rectTransform.anchoredPosition = parent.Design.initialPosition + Offset;
    }

    protected override void UnHover()
    {
        if (!isSelect)
        {
            isHover = false;
            parent.rectTransform.sizeDelta = parent.initialSize;
            parent.Design.rectTransform.anchoredPosition = parent.Design.initialPosition;
        }
    }

    protected override void Push()
    {

    }

    protected override void Release()
    {
        if (isHover)
        {

        }
    }

    public void MyPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isSelect)
            {
                cardSelectEvent.Add.OnNext(parent.transform.GetSiblingIndex());
                rectTransform.anchoredPosition = initialPosition + Offset;
            }
            else
            {
                cardSelectEvent.Invoke.OnNext(true);
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right && isSelect)
        {
            cardSelectEvent.Remove.OnNext(parent.transform.GetSiblingIndex());
            rectTransform.anchoredPosition = initialPosition;
        }

    }
}
