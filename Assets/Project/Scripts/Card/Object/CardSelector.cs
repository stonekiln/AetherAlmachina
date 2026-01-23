using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using DIVFactor.Injectable;
using DConfig.EntityLife.Event;

/// <summary>
/// カードを画面から選択するためのクラス
/// </summary>
public class CardSelector : ButtonBase, IInjectable
{
    CardActivateEventBundle CardActivate;
    CardBase parent;
    RectTransform rectTransform;
    Vector2 initialPosition;
    readonly Vector2 ExtraSpacing = new(40f, 0);
    readonly Vector2 Offset = new(0, 20f);

    public void Injection(InjectableResolver resolver)
    {
        resolver.Inject(out CardActivate);
    }
    public void Initialize(CardBase cardBase)
    {
        parent = cardBase;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        onClickCallback = () => cardBase.SkillData.Activate();
    }
    void OnEnable()
    {
        OnPointerClickAsObservable().Subscribe(eventData => MyPointerClick(eventData)).AddTo(this);
        OnPointerDownAsObservable().Subscribe(eventData => Push()).AddTo(this);
        OnPointerUpAsObservable().Subscribe(eventData => Release()).AddTo(this);
        OnPointerEnterAsObservable().Subscribe(eventData => Hover()).AddTo(this);
        OnPointerExitAsObservable().Subscribe(eventData => UnHover()).AddTo(this);
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
                CardActivate.Select.Publish(new(parent, parent.transform.GetSiblingIndex()));
                rectTransform.anchoredPosition = initialPosition + Offset;
            }
            else
            {
                CardActivate.Invoke.Publish(new());
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right && isSelect)
        {
            CardActivate.Cancel.Publish(new(parent, parent.transform.GetSiblingIndex()));
            rectTransform.anchoredPosition = initialPosition;
        }

    }
}
