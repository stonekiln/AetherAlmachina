using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Faction;
using AetherAlmachina.Entities.Parameter;
using DConfig.EnemyLife;
using DConfig.PlayerLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using R3;
using UnityEngine;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// エンティティをスポーンさせるスポナー
    /// </summary>
    public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
    {
        /// <summary>
        /// スポーンできるエンティティの情報を渡す
        /// </summary>
        /// <param name="Friendly">友好的なエンティティ</param>
        /// <param name="Hostile">敵対的なエンティティ</param>
        //友好敵対の基準はプレイヤー
        public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
        Func<StatusAsset, Player> playerFactory;
        Func<StatusAsset, Enemy> enemyFactory;
        EntityList Data;
        List<IEntityInteraction> friendlyEntity;
        List<IEntityInteraction> hostileEntity;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out StageSettingsAsset settings);
            Data = new(new StatusAsset[] { settings.Player }.Concat(settings.Friendly).ToArray(), settings.Hostile);
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
            friendlyEntity = new List<IEntityInteraction>() { playerFactory(Data.Friendly[0]) };
            hostileEntity = new List<IEntityInteraction>(Data.Hostile.Select(asset => enemyFactory(asset)));
            SetUpTargeting(friendlyEntity, hostileEntity);
        }

        /// <summary>
        /// ターゲティングができるように各種イベントの購読を行う
        /// </summary>
        /// <param name="friendlyEntity">友好的なエンティティ</param>
        /// <param name="hostileEntity">敵対的なエンティティ</param>
        void SetUpTargeting(List<IEntityInteraction> friendlyEntity, List<IEntityInteraction> hostileEntity)
        {
            friendlyEntity.ForEach(friendly => friendly.Targeting.LockOn.Reply(req => new(req.Selector(friendlyEntity, hostileEntity))).AddTo(this));
            hostileEntity.ForEach(hostile => hostile.Targeting.LockOn.Reply(req => new(req.Selector(hostileEntity, friendlyEntity))).AddTo(this));
        }
    }
}