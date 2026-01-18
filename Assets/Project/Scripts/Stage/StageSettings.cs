using UnityEngine;

/// <summary>
/// ステージの設定をするためのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "StageSettings", menuName = "GameSettings/StageSettings")]
public class StageSettings : ScriptableObject
{
    [SerializeField] private CostSettings costSettings;
    [SerializeField] private StatusAsset player;
    [SerializeField] private StatusAsset[] friendly;
    [SerializeField] private StatusAsset[] hostile;

    public CostSettings CostSettings => costSettings;
    public StatusAsset Player => player;
    public StatusAsset[] Friendly => friendly;
    public StatusAsset[] Hostile => hostile;
}