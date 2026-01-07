using System;
using DivFacter.Injectable;
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

    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out design);
        resolver.Inject(out selector);
        resolver.RegisterBinder(this);
    }

    public CardBase Initialize(SkillData data)
    {
        skillData = data;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
        design.Initialize(this);
        selector.Initialize(this);
        return this;
    }

    public ICardData SetCard(int index)
    {
        transform.SetSiblingIndex(index);
        return this;
    }
    public ICardData RemoveCard()
    {
        Destroy(this);
        return this;
    }

    public void SetSelect(bool flag)
    {
        Selector.isSelect = flag;
    }
}