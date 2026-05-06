using System;
using System.IO;
using DConfig.StageLife;
using DIVFactor.Spawner;
using UnityEngine;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// ステージをスポーンさせるスポナー
    /// </summary>
    public class StageSpawner : MonoBehaviour, ILifetimeSpawner
    {
        [field: SerializeField] StageSettingsAsset StageSettings { get; set; }
        Action<StageSettingsAsset> spawner;

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.Register<StageLifetime>(Resources.Load<GameObject>(Path.Combine("Stage", "Debug")))
                    .Inject(out spawner);
        }
        void Start()
        {
            spawner(StageSettings);
        }
    }
}