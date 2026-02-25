using System;
using System.IO;
using DConfig.StageLife;
using DIVFactor.Spawner;
using UnityEngine;

public class StageManager : MonoBehaviour, ILifetimeSpawner
{
    [field: SerializeField] StageSettings StageSettings { get; set; }
    Action<StageSettings> spawner;

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