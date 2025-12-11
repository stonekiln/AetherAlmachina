using R3;
using TMPro;
using UnityEngine;

/// <summary>
/// あるエンティティのコストを表示するためのクラス
/// </summary>
public class CostDisplay : MonoBehaviour
{
    [SerializeField] MonitoredEntity mpMonitoringEntity;
    TextMeshProUGUI textMeshPro;

    void Awake()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
    }
    void OnEnable()
    {
        mpMonitoringEntity.magicPoint.Subscribe(mp => SetDisplay(mp)).AddTo(this);
    }

    void SetDisplay(int mp)
    {
        textMeshPro.text = mp.ToString();
    }
}