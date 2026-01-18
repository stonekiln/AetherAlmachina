using System;
using System.IO;
using System.Linq;
using DConfig.EnemyLife;
using DConfig.PlayerLife;
using DivFacter.Injectable;
using DivFacter.Lifetime;
using UnityEngine;
using Utility;

public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
{
    Action<StatusAsset> playerFactory;
    Action<StatusAsset> enemyFactory;
    EntityList Data;
    public void SpawnConfigure(ObjectBuilder builder)
    {
        builder.MakeSpawner<PlayerLifetime>(Resources.Load<GameObject>(Path.Combine("EntityObject", "PlayerObject")))
                .Get(out playerFactory, (asset, register) => register.Asset(asset));

        builder.MakeSpawner<EnemyLifetime>(Resources.Load<GameObject>(Path.Combine("EntityObject", "EnemyObject")))
                .Get(out enemyFactory, (asset, register) => register.Asset(asset));
    }
    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out StageSettings settings);
        Data = new(settings.Friendly.Append(settings.Player).Reverse().ToArray(), settings.Hostile);
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