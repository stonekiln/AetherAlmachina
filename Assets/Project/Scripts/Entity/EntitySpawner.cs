using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DConfig.EnemyLife;
using DConfig.EntityLife.Event;
using DConfig.PlayerLife;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using R3;
using UnityEngine;

public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
{
    public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
    Func<StatusAsset, EventBus<TargetingEvent>> playerFactory;
    Func<StatusAsset, EventBus<TargetingEvent>> enemyFactory;
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
        Observable<TargetingEvent> FriendlyTargetingEvent = playerFactory(Data.Friendly[0]).AsObservable();
        Observable<TargetingEvent> HostileTargetingEvent = Observable.Merge(Data.Hostile.Select(asset => enemyFactory(asset)));
        FriendlyTargetingEvent.Subscribe(target =>
        {

        }).AddTo(this);
    }
}