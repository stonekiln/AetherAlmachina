using System;
using DIVFactor.Injectable;
using UnityEngine;

/// <summary>
/// カードのオブジェクトの親オブジェクトとなるクラス
/// </summary>
public class CardBase : MonoBehaviour, ICardData, IInjectable
{
    CardDesign design;
    CardSelector selector;
    public CardDesign Design => design;
    public CardSelector Selector => selector;
    SkillData skillData;
    public SkillData SkillData => skillData;
    public bool IsSelect => Selector.isSelect;
    Action EntryEndPoint;

    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    public void Injection(InjectableResolver resolver)
    {
        resolver.Inject(out skillData);
        resolver.Inject(out design);
        resolver.Inject(out selector);
        EntryEndPoint = resolver.EntryEndPoint;

        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
        design.Initialize(this);
        selector.Initialize(this);
    }

    public ICardData SetCard(int index)
    {
        transform.SetSiblingIndex(index);
        return this;
    }
    public ICardData RemoveCard()
    {
        EntryEndPoint();
        return this;
    }

    public void SetSelect(bool flag)
    {
        Selector.isSelect = flag;
    }
}