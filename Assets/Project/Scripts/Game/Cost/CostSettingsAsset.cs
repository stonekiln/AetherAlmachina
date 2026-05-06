using UnityEngine;

namespace AetherAlmachina.Cost
{
    /// <summary>
    /// ステージ毎のコストの設定を保持する
    /// </summary>
    [CreateAssetMenu(fileName = "CostSettings", menuName = "GameSettings/CostSettings")]
    public class CostSettingsAsset : ScriptableObject
    {
        /// <summary>
        /// コストの増加量
        /// </summary>
        [field: SerializeField] public int Delta { get; private set; }
        /// <summary>
        /// コストの増加頻度(sec)
        /// </summary>
        [field: SerializeField] public float TimeSpan { get; private set; }
    }
}