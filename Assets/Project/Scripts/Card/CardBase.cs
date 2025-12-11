using System;
using DivFacter.Injectable;
using UnityEngine;
using VContainer;

/// <summary>
/// カードのオブジェクトの親オブジェクトとなるクラス
/// </summary>
public class CardBase : MonoBehaviour, IInjectable
{
    CardDesign design;
    CardSelecter selecter;
    public CardDesign Design => design;
    public CardSelecter Selecter => selecter;
    public SkillData Data { get; private set; }
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out design);
        resolver.Inject(out selecter);
    }

    public void Initialize(SkillData data)
    {
        Data = data;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
        design.Initialize(this);
        selecter.Initialize(this);
    }
}