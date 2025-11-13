using UnityEngine;
using UnityEngine.UI;

public class CardDesign : MonoBehaviour
{
    CardManager cardData;
    Image image;

    public void Initialize(CardManager cardManager,Sprite icon)
    {
        cardData = cardManager;
        image = gameObject.GetComponent<Image>();
        image.sprite = icon;
    }
}
