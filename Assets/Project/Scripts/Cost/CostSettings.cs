using UnityEngine;

/// <summary>
/// ステージ毎のコストの設定を設定するためのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "CostSettings", menuName = "GameSettings/CostSettings")]
public class CostSettings : ScriptableObject
{
    [field: SerializeField] public int Delta { get; private set; }
    [field: SerializeField] public float TimeSpan { get; private set; }
}