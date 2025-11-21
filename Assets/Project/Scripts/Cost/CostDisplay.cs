using R3;
using TMPro;
using UnityEngine;

public class CostDisplay : MonoBehaviour
{
    [SerializeField] MonitoredEntity mpMonitoringEntity;
    TextMeshProUGUI textMeshPro;
    void Start()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        mpMonitoringEntity.magicPoint.Subscribe(mp => SetDisplay(mp)).AddTo(this);
    }

    void SetDisplay(int mp)
    {
        textMeshPro.text = mp.ToString();
    }
}