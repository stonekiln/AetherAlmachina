using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カードの外見を設定するためのクラス
/// </summary>
public class CardDesign : MonoBehaviour
{
    CardBase parent;
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialPosition;
    Image image;
    
    public void Initialize(CardBase cardManager)
    {
        parent = cardManager;
        rectTransform = gameObject.GetComponent<RectTransform>();
        image = gameObject.GetComponent<Image>();
        image.sprite = parent.SkillData.Icon;
    }
}
