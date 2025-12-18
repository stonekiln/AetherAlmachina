using System;
using DivFacter.Injectable;
using UnityEngine;

/// <summary>
/// カードのオブジェクトの親オブジェクトとなるクラス
/// </summary>
public class CardBase : MonoBehaviour, ICardData, IInjectable
{
    CardDesign design;
    CardSelecter selecter;
    public CardDesign Design => design;
    public CardSelecter Selecter => selecter;
    SkillData skillData;
    public SkillData SkillData => skillData;
    public bool IsSelect { get; set; }

    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out design);
        resolver.Inject(out selecter);
    }

    public CardBase Initialize(SkillData data)
    {
        skillData = data;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
        design.Initialize(this);
        selecter.Initialize(this);
        return this;
    }

    public ICardData SetCard(int index)
    {
        transform.SetSiblingIndex(index);
        return this;
    }
    public ICardData RemomveCard()
    {
        Destroy(this);
        return this;
    }
}