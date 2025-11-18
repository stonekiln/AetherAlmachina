using R3;
using TMPro;
using UnityEngine;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] MonitoredEntity monitoredEntity;
    TextMeshProUGUI textMeshPro;
    void Start()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        monitoredEntity.magicPoint.Subscribe(mp => SetDisplay(mp)).AddTo(this);
    }

    void SetDisplay(int mp)
    {
        textMeshPro.text = mp.ToString();
    }
}