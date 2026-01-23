using System;
using System.IO;
using System.Linq;
using DConfig.EnemyLife;
using DConfig.PlayerLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using UnityEngine;

public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
{
    Action<StatusAsset> playerFactory;
    Action<StatusAsset> enemyFactory;
    EntityList Data;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out StageSettings settings);
        Data = new(settings.Friendly.Append(settings.Player).Reverse().ToArray(), settings.Hostile);
    }
    public void SpawnConfigure(SpawnerBuilder builder)
    {
        builder.Register<PlayerLifetime>(Resources.Load<GameObject>(Path.Combine("EntityObject", "PlayerObject")))
                .Inject(out playerFactory);
        builder.Register<EnemyLifetime>(Resources.Load<GameObject>(Path.Combine("EntityObject", "EnemyObject")))
                .Inject(out enemyFactory);
    }

    void Start()
    {
        playerFactory(Data.Friendly[0]);
        foreach (StatusAsset asset in Data.Hostile)
        {
            enemyFactory(asset);
        }
    }
}