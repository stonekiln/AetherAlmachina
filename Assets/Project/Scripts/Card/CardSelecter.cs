using DG.Tweening;
using DConfig.Battle.Event;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using VContainer;

/// <summary>
/// カードを画面から選択するためのクラス
/// </summary>
public class CardSelecter : ButtonBase
{
    [Inject] CardActivateEventBundle CardActive;
    CardBase parent;
    RectTransform rectTransform;
    Vector2 initialPosition;
    readonly Vector2 ExtraSpacing = new(40f, 0);
    readonly Vector2 Offset = new(0, 20f);

    void OnEnable()
    {
        OnPointerClickAsObservable().Subscribe(eventData => MyPointerClick(eventData)).AddTo(this);
        OnPointerDownAsObservable().Subscribe(eventData => Push()).AddTo(this);
        OnPointerUpAsObservable().Subscribe(eventData => Release()).AddTo(this);
        OnPointerEnterAsObservable().Subscribe(eventData => Hover()).AddTo(this);
        OnPointerExitAsObservable().Subscribe(eventData => UnHover()).AddTo(this);
    }

    public void Initialize(CardBase cardBase)
    {
        Debug.Log(cardBase);
        parent = cardBase;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        onClickCallback = () => cardBase.Data.Activate();
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
        Debug.Log(parent);
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
                CardActive.Select.Publish(new(parent.transform.GetSiblingIndex()));
                rectTransform.anchoredPosition = initialPosition + Offset;
            }
            else
            {
                CardActive.Invoke.Publish(new());
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right && isSelect)
        {
            CardActive.Cancel.Publish(new(parent.transform.GetSiblingIndex()));
            rectTransform.anchoredPosition = initialPosition;
        }

    }
}
