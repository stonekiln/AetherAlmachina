using DIVFactor.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Faction;
using AetherAlmachina.Entities.Parameter;
using DConfig.EnemyLife;
using DConfig.EntityLife.Event;
using DConfig.PlayerLife;
using DConfig.StageLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using R3;
using UnityEngine;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// エンティティをスポーンさせるスポナー
    /// </summary>
    public class EntitySpawner : MonoBehaviour, IActiveComponent, ILifetimeSpawner
    {
        /// <summary>
        /// スポーンできるエンティティの情報を渡す
        /// </summary>
        /// <param name="Friendly">友好的なエンティティ</param>
        /// <param name="Hostile">敵対的なエンティティ</param>
        //友好敵対の基準はプレイヤー
        public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
        [Mold("EntityObject/PlayerMock")]
        Factory<StatusAsset, Player> playerFactory;
        [Mold("EntityObject/EnemyMock")]
        Factory<StatusAsset, Enemy> enemyFactory;
        EntityList Data;
        StageSettingsAsset settings;
        UsageLifetime targeting;
        List<Entity> friendlyEntity;
        List<Entity> hostileEntity;
        EventPort<FriendlyLayoutEvent> friendlyLayoutEvent;
        EventPort<HostileLayoutEvent> hostileLayoutEvent;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out settings);

            resolver.Inject(out friendlyLayoutEvent);
            resolver.Inject(out hostileLayoutEvent);
        }
        public void Initialize() => Data = new(settings.Friendly.Entities, settings.Hostile.Entities);

        public void SpawnConfigure(SpawnerBuilder builder)
        {
            builder.Register<PlayerLifetime>()
                    .Inject(out playerFactory);
            builder.Register<EnemyLifetime>()
                    .Inject(out enemyFactory);
        }
        public void Subscribe(UsageLifetime lifetime) => targeting = lifetime;

        public void Activate()
        {
            friendlyEntity = Data.Friendly.Select(asset => (Entity)playerFactory.Spawn(asset)).ToList();
            hostileEntity = Data.Hostile.Select(asset => (Entity)enemyFactory.Spawn(asset)).ToList();

            friendlyLayoutEvent.OnNext(new(friendlyEntity));
            hostileLayoutEvent.OnNext(new(hostileEntity));

            SetUpTargeting(friendlyEntity, hostileEntity);
        }

        /// <summary>
        /// ターゲティングができるように各種イベントの購読を行う
        /// </summary>
        /// <param name="friendlyEntity">友好的なエンティティ</param>
        /// <param name="hostileEntity">敵対的なエンティティ</param>
        void SetUpTargeting(List<Entity> friendlyEntity, List<Entity> hostileEntity)
        {
            void SetLockOn(LockOnEventHub lockOn) => lockOn.AsObservable()
                .Subscribe(log => new(log.Selector(friendlyEntity, hostileEntity))).AddTo(targeting);

            friendlyEntity.ForEach(friendly => SetLockOn(friendly.LockOn));
            hostileEntity.ForEach(hostile => SetLockOn(hostile.LockOn));
        }
    }
}
