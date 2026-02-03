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
using SKill;
using UnityEngine;

public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
{
    public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
    Func<StatusAsset, AttackEventBundle> playerFactory;
    Func<StatusAsset, AttackEventBundle> enemyFactory;
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
        List<AttackEventBundle> friendlyAttackEvent = new() { playerFactory(Data.Friendly[0]) };
        List<AttackEventBundle> hostileAttackEvent = new(Data.Hostile.Select(asset => enemyFactory(asset)));
        Observable<TargetingEvent> friendlyTargeting = Observable.Merge(friendlyAttackEvent.Select(attack => attack.Targeting));
        List<EventBus<HitEvent>> friendlyHit = friendlyAttackEvent.Select(attack => attack.Hit).ToList();
        Observable<TargetingEvent> hostileTargeting = Observable.Merge(hostileAttackEvent.Select(attack => attack.Targeting));
        List<EventBus<HitEvent>> hostileHit = hostileAttackEvent.Select(attack => attack.Hit).ToList();

        friendlyTargeting.Subscribe(log =>
        {
            if (log.Data.Target.HasFlag(TargetFlag.Friendly))
            {
                foreach (EventBus<HitEvent> hit in friendlyHit.Take(log.Data.MaxTargeting))
                {
                    hit.OnNext(new(log.Data.Activate));
                }
            }
            if (log.Data.Target.HasFlag(TargetFlag.Hostile))
            {
                foreach (EventBus<HitEvent> hit in hostileHit.Take(log.Data.MaxTargeting))
                {
                    hit.OnNext(new(log.Data.Activate));
                }
            }
        }).AddTo(this);
        hostileTargeting.Subscribe(log =>
        {
            if (log.Data.Target.HasFlag(TargetFlag.Friendly))
            {
                foreach (EventBus<HitEvent> hit in hostileHit.Take(log.Data.MaxTargeting))
                {
                    hit.OnNext(new(log.Data.Activate));
                }
            }
            if (log.Data.Target.HasFlag(TargetFlag.Hostile))
            {
                foreach (EventBus<HitEvent> hit in friendlyHit.Take(log.Data.MaxTargeting))
                {
                    hit.OnNext(new(log.Data.Activate));
                }
            }
        }).AddTo(this);
    }
}