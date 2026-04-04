using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AetherAlmachina.Entities.Faction;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Stage;
using DConfig.EnemyLife;
using DConfig.PlayerLife;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using R3;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    public class EntitySpawner : MonoBehaviour, IInjectable, ILifetimeSpawner
    {
        public record EntityList(StatusAsset[] Friendly, StatusAsset[] Hostile);
        Func<StatusAsset, Player> playerFactory;
        Func<StatusAsset, Enemy> enemyFactory;
        EntityList Data;
        List<ICombatInteraction> friendlyInteractions;
        List<ICombatInteraction> hostileInteractions;

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
            var friends = Data.Friendly.Select(asset => playerFactory(asset)).ToList();
            var enemies = Data.Hostile.Select(asset => enemyFactory(asset)).ToList();

            AlignPosition(friends, new(8f, 0.1f, 0f), 1);
            AlignPosition(enemies, new(-8f, 0.1f, 0f), 2, direction: -1);

            friendlyInteractions = new List<ICombatInteraction>(friends);
            hostileInteractions = new List<ICombatInteraction>(enemies);
            SetUpTargeting(friendlyInteractions, hostileInteractions);
        }

        void SetUpTargeting(List<ICombatInteraction> friendlyInteractions, List<ICombatInteraction> hostileInteractions)
        {
            friendlyInteractions.ForEach(friendly => friendly.Targeting.LockOn.Reply(req => new(req.Selector(friendlyInteractions, hostileInteractions))).AddTo(this));
            hostileInteractions.ForEach(hostile => hostile.Targeting.LockOn.Reply(req => new(req.Selector(hostileInteractions, friendlyInteractions))).AddTo(this));
        }

        // TODO: 領域のテスト描画を行えるようにする
        // TODO: columnSize は、あくまでも最大値ということにして entities.Count がそれより小さいときはそれに合わせるようにするか、
        //       entities.Count からある程度適切な列数を計算できるようにするか
        /// <summary>
        /// 複数のエンティティを origin を中心とした領域に、指定した列数 (columnSize) に揃えて配置する
        /// デフォルトで右方向に揃える
        /// </summary>
        /// <typeparam name="TEntity">エンティティ型</typeparam>
        /// <param name="entities">エンティティのリスト</param>
        /// <param name="origin">配置の中心点</param>
        /// <param name="columnSize">列数</param>
        /// <param name="direction">
        /// x 軸（z 軸の正方向が、<b>カメラ正面 = 画面奥</b> 側のときの、左右方向）における配置方向
        /// 1 の時は origin から見て右側に配置、-1 の時は左側に配置（デフォルト 1）
        /// </param>
        void AlignPosition<TEntity>(List<TEntity> entities, Vector3 origin, uint columnSize, int direction = 1)
            where TEntity : Entity
        {
            // 列数から行数を計算
            uint rowSize = (uint)Mathf.Ceil(entities.Count / (float)columnSize);

            // 左右方向のマージン
            // 画面奥側のエンティティは手前側よりもちょっと左右に移動させる
            const float Margin = 0.5f;

            // 台の大きさを取得 (仮)
            MeshRenderer field = GameObject.Find("Field").GetComponent<MeshRenderer>();;

            // 配置領域のデータ (仮)
            var region = new
            {
                // 領域の幅：スクリーン幅
                Width = field.bounds.size.x / 3f,

                // 領域の幅：台 (field) の奥行き
                Depth = field.bounds.size.z
            };

            // 配置位置の計算
            // 配置領域（矩形）の原点は、左下を (0, 0) の基準とする
            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < columnSize; j++)
                {
                    // 操作するエンティティのインデックス
                    // i 行 j 列のエンティティのインデックスは、i * 列数 + j
                    int index = i * (int)columnSize + j;

                    if (index >= entities.Count)
                    {
                        break;
                    }
                    
                    entities[index].transform.position = new Vector3
                    {
                        // x 座標は、配置方向とマージンを加味する
                        x = direction * (i / (float)rowSize * region.Width + j * Margin),
                        z = j / (float)columnSize * region.Depth
                    };
                }
            }

            // 領域（台）から origin への位置の移動量
            // 変化後 - 変化前
            // 基準を 0 にして計算したものは変化量なし、直接指定
            Vector3 deltaCenter = new()
            {
                // 上の for 文で配置した位置は、i も j も 0 スタートの列 or 行数未満で、
                // region の基準から配置されて行くので、x 方向は region.Width / rowSize
                // z 方向は region.Depth / columnSize だけ余分な幅がある
                // そのため中心座標はそれらを引いてから計算する
                x = origin.x - direction * (region.Width - region.Width / rowSize) / 2f,
                y = origin.y,
                z = origin.z - (region.Depth - region.Depth / columnSize) / 2f
            };

            // 位置の変化量だけ移動させると、origin を中心とした配置になる
            foreach (var entity in entities)
            {
                entity.transform.position += deltaCenter;
            }
        }
    }
}
