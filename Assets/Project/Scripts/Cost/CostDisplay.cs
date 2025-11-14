using R3;
using TMPro;
using UnityEngine;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] GameObject monitored;
    TextMeshProUGUI textMeshPro;
    void Start()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        monitored.GetComponent<Entity>().magicPoint.Subscribe(mp => SetDisplay(mp)).AddTo(this);
    }

    void SetDisplay(int mp)
    {
        textMeshPro.text = mp.ToString();
    }
}