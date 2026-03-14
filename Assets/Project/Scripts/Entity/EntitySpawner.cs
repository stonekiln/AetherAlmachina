using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DConfig.EnemyLife;
using DConfig.EntityLife.Event;
using DConfig.PlayerLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using R3;
using UnityEngine;
using Utility;

public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
{
    public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
    Func<StatusAsset, Player> playerFactory;
    Func<StatusAsset, Enemy> enemyFactory;
    EntityList Data;
    List<ICombatInteraction> friendlyEntity;
    List<ICombatInteraction> hostileEntity;

    public void Injection(InjectableResolver resolver)
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
        friendlyEntity = new List<ICombatInteraction>() { playerFactory(Data.Friendly[0]) };
        hostileEntity = new List<ICombatInteraction>(Data.Hostile.Select(asset => enemyFactory(asset)));
        SetUpTargeting(friendlyEntity, hostileEntity);
    }

    void SetUpTargeting(List<ICombatInteraction> friendlyEntity, List<ICombatInteraction> hostileEntity)
    {
        friendlyEntity.ForEach(friendly => friendly.AttackEvent.Targeting.Reply(req => new(req.TargetSetter(friendlyEntity, hostileEntity))).AddTo(this));
        hostileEntity.ForEach(hostile => hostile.AttackEvent.Targeting.Reply(req => new(req.TargetSetter(hostileEntity, friendlyEntity))).AddTo(this));
    }
}