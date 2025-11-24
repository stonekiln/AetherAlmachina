using System;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardDesign Design { get; private set; }
    public CardSelecter Selecter { get; private set; }
    public SkillData Data { get; private set; }
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialSize;

    void Awake()
    {
        Design = transform.GetChild(0).GetComponent<CardDesign>();
        Selecter = transform.GetChild(1).GetComponent<CardSelecter>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = rectTransform.rect.size;
    }

    public void Initialize(SkillData skillData)
    {
        Data = skillData;
        Selecter.Initialize(this);
        Design.Initialize(this);
    }
}