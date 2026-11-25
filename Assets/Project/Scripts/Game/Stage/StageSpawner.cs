using System;
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
        [Mold("Stage/Debug")]
        ActionFactory<StageSettingsAsset> spawner;

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.Register<StageLifetime>()
                    .Inject(out spawner);
        }
        void Start()
        {
            spawner.Spawn(StageSettings);
        }
    }
}
