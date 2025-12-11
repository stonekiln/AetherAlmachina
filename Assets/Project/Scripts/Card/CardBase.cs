using System;
using UnityEngine;
using VContainer;

/// <summary>
/// カードのオブジェクトの親オブジェクトとなるクラス
/// </summary>
public class CardBase : MonoBehaviour
{
    public CardDesign Design { get; private set; }
    public CardSelecter Selecter { get; private set; }
    public SkillData Data { get; private set; }
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    [Inject]
    void Construct(CardDesign cardDesign, CardSelecter cardSelecter)
    {
        Design = cardDesign;
        Selecter = cardSelecter;
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
    }

    public void SetSkilldata(SkillData data)
    {
        Data = data;
    }
}