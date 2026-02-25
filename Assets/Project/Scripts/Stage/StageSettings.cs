using UnityEngine;

/// <summary>
/// ステージの設定をするためのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "StageSettings", menuName = "GameSettings/StageSettings")]
public class StageSettings : ScriptableObject
{
    [field: SerializeField] public CostSettings CostSettings { get; private set; }
    [field: SerializeField] public StatusAsset Player { get; private set; }
    [field: SerializeField] public StatusAsset[] Friendly { get; private set; }
    [field: SerializeField] public StatusAsset[] Hostile { get; private set; }
}