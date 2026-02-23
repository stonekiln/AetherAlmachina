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

public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
{
    public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
    Func<StatusAsset, Player> playerFactory;
    Func<StatusAsset, Enemy> enemyFactory;
    EntityList Data;

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
        IEnumerable<ICombatInteraction> friendlyEntity = new List<ICombatInteraction>() { playerFactory(Data.Friendly[0]) };
        IEnumerable<ICombatInteraction> hostileEntity = new List<ICombatInteraction>(Data.Hostile.Select(asset => enemyFactory(asset)));
        SetUpTargeting(friendlyEntity, hostileEntity);
    }

    void SetUpTargeting(IEnumerable<ICombatInteraction> friendlyEntity, IEnumerable<ICombatInteraction> hostileEntity)
    {
        Observable<TargetingEvent> friendlyTargeting = Observable.Merge(friendlyEntity.Select(entity => entity.AttackEvent.Targeting));
        Observable<TargetingEvent> hostileTargeting = Observable.Merge(hostileEntity.Select(entity => entity.AttackEvent.Targeting));

        friendlyTargeting.Subscribe(log =>
        {
            log.Data.Targeting(friendlyEntity, hostileEntity);
        }).AddTo(this);
        hostileTargeting.Subscribe(log =>
        {
            log.Data.Targeting(hostileEntity, friendlyEntity);
        }).AddTo(this);
    }
}