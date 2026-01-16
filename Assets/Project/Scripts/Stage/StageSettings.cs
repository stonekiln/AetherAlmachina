using UnityEngine;

/// <summary>
/// ステージの設定をするためのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "StageSettings", menuName = "GameSettings/StageSettings")]
public class StageSettings : ScriptableObject
{
    [SerializeField] private CostSettings costSettings;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] friendly;
    [SerializeField] private GameObject[] hostile;

    public CostSettings CostSettings => costSettings;
    public GameObject Player => player;
    public GameObject[] Friendly => friendly;
    public GameObject[] Hostile => hostile;
}