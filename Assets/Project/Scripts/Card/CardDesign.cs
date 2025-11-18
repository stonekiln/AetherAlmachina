using System;
using UnityEngine;
using UnityEngine.UI;

public class CardDesign : MonoBehaviour
{
    CardManager cardData;
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialPosition;
    Image image;

    public void Initialize(CardManager cardManager, Sprite icon)
    {
        cardData = cardManager;
        rectTransform = gameObject.GetComponent<RectTransform>();
        image = gameObject.GetComponent<Image>();
        image.sprite = icon;
    }
}
