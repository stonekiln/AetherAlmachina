using UnityEngine;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
    int cost;
    public Image image;

    public void Initialize(Sprite icon, int cardCost)
    {
        image = gameObject.GetComponent<Image>();
        image.sprite = icon;
        cost = cardCost;
    }
}
