using AetherAlmachina.Cost;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// ステージの設定を保持する
    /// </summary>
    [CreateAssetMenu(fileName = "StageSettings", menuName = "GameSettings/StageSettings")]
    public class StageSettingsAsset : ScriptableObject
    {
        [field: SerializeField] public CostSettingsAsset CostSettings { get; private set; }
        [field: SerializeField] public StatusAsset Player { get; private set; }
        [field: SerializeField] public StatusAsset[] Friendly { get; private set; }
        [field: SerializeField] public StatusAsset[] Hostile { get; private set; }
    }
}