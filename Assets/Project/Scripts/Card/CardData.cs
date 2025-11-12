using UnityEngine;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
    public int Cost{ get; private set; }
    public Image image;

    public void Initialize(Sprite icon, int cardCost)
    {
        image = gameObject.GetComponent<Image>();
        image.sprite = icon;
        Cost = cardCost;
    }
}
