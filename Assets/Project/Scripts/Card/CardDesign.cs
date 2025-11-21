using System;
using UnityEngine;
using UnityEngine.UI;

public class CardDesign : MonoBehaviour
{
    CardManager parent;
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public Vector2 initialPosition;
    Image image;

    public void Initialize(CardManager cardManager)
    {
        parent = cardManager;
        rectTransform = gameObject.GetComponent<RectTransform>();
        image = gameObject.GetComponent<Image>();
        image.sprite = parent.Data.Icon;
    }
}
