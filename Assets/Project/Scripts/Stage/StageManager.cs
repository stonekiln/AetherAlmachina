using System;
using System.IO;
using DConfig.StageLife;
using DivFacter.Lifetime;
using UnityEngine;
using VContainer;

public class BattleManager : MonoBehaviour, ILifetimeSpawner
{
    [SerializeField] StageSettings stageSettings;
    Func<StageSettings, CostManager> spawner;

    public void SpawnConfigure(ObjectBuilder builder)
    {
        builder.Set<StageLifetime, StageSettings, CostManager>(out spawner, Resources.Load<GameObject>(Path.Combine("Stage", "Debug")),
            (stageSettings, resolver) =>
            {
                resolver.Resolve<CostManager>().Initialize(stageSettings.CostSettings);
            }
        );
    }
    void Start()
    {
        spawner(stageSettings);
    }
}