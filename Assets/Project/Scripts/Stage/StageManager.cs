using System;
using System.IO;
using System.Linq;
using DConfig.StageLife;
using DivFacter.Lifetime;
using UnityEngine;
using VContainer;

public class StageManager : MonoBehaviour, ILifetimeSpawner
{
    [SerializeField] StageSettings stageSettings;
    Action<StageSettings> spawner;

    public void SpawnConfigure(ObjectBuilder builder)
    {
        builder.MakeSpawner<StageLifetime>(Resources.Load<GameObject>(Path.Combine("Stage", "Debug")))
                .Get(out spawner, (setting, register) => register.Asset(setting));
    }
    void Start()
    {
        spawner(stageSettings);
    }
}