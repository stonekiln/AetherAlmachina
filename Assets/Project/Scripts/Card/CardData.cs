using UnityEngine;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
    int cost;
    Image image;

    public void Initialize(Sprite icon, int cardCost)
    {
        Debug.Log(icon.name);
        image = gameObject.GetComponent<Image>();
        image.sprite = icon;
        cost = cardCost;
    }
}
