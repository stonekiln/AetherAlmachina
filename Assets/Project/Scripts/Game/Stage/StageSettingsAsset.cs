using System;
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
        [Serializable]
        public class EntitySpawnData
        {
            /// <summary>
            /// エンティティを配置する列数と行数
            /// xが列 (columns)、yが行数 (rows) を表す
            /// </summary>
            public Vector2Int layoutSize;
            [field: SerializeField] public StatusAsset[] Entities { get; private set; }
        }

        [field: SerializeField] public CostSettingsAsset CostSettings { get; private set; }
        [field: SerializeField] public EntitySpawnData Friendly { get; private set; }
        [field: SerializeField] public EntitySpawnData Hostile { get; private set; }
    }
}
