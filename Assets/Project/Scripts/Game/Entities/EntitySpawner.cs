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
        [SerializeField] GameObject field;

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
            // 仮取得
            field = GameObject.Find("Field");

            // 今は台座の想定が Field オブジェクトなので、ややこしいことはしないでおく
            // var a = GetFieldPointFromViewport(new(0.33f, 0.5f), out field);
            // var b = GetFieldPointFromViewport(new(0.66f, 0.5f), out _);

            var friends = Data.Friendly.Select(asset => playerFactory(asset)).ToList();
            var enemies = Data.Hostile.Select(asset => enemyFactory(asset)).ToList();

            // 奥行き方向の列数は、エンティティの数の平方根の切り上げにしておく（仮？）
            AlignPosition(friends, new(-4f, 0.1f, 0f), (uint)Mathf.Ceil(Mathf.Sqrt(friends.Count)), direction: -1);
            AlignPosition(enemies, new(4f, 0.1f, 0f), (uint)Mathf.Ceil(Mathf.Sqrt(enemies.Count)), direction: 1);
            
            friendlyInteractions = new List<ICombatInteraction>(friends);
            hostileInteractions = new List<ICombatInteraction>(enemies);
            SetUpTargeting(friendlyInteractions, hostileInteractions);
        }

        void SetUpTargeting(List<ICombatInteraction> friendlyInteractions, List<ICombatInteraction> hostileInteractions)
        {
            friendlyInteractions.ForEach(friendly => friendly.Targeting.LockOn.Reply(req => new(req.Selector(friendlyInteractions, hostileInteractions))).AddTo(this));
            hostileInteractions.ForEach(hostile => hostile.Targeting.LockOn.Reply(req => new(req.Selector(hostileInteractions, friendlyInteractions))).AddTo(this));
        }

        // 今はまだ使わない
        /// <summary>
        /// スクリーン座標から、台座 (field) を取得する
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <param name="field"></param>
        /// <returns>指定したスクリーン座標に対応する台座上の位置（ワールド座標）</returns>
        Vector3 GetFieldPointFromViewport(Vector2 screenPoint, out GameObject field)
        {
            Ray ray = Camera.main.ViewportPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Field")))
            {
                field = hit.collider.gameObject;
                return hit.point;
            }

            field = null;
            return Vector3.zero;
        }

        /// <summary>
        /// 複数のエンティティを origin を中心とした領域に、指定した列数 (columnSize) に揃えて配置する
        /// デフォルトで右方向に揃える
        /// </summary>
        /// <typeparam name="TEntity">エンティティ型</typeparam>
        /// <param name="entities">エンティティのリスト</param>
        /// <param name="origin">配置の中心点</param>
        /// <param name="columnSize">列数 Z方向（奥行き方向）の配置数</param>
        /// <param name="direction">
        /// X軸（Z軸の正方向が、<b>カメラ正面 = 画面奥</b> 側のときの、左右方向）における配置方向
        /// 1 の時は origin から見て右側に配置、-1 の時は左側に配置（デフォルト 1）
        /// </param>
        /// <param name="isDrawRegion">配置領域を描画するかどうか（デバッグ用）</param>
        void AlignPosition<TEntity>(List<TEntity> entities, Vector3 origin, uint columnSize, int direction = 1, bool isDrawRegion = false)
            where TEntity : Entity
        {
            // 列数から行数を計算
            uint rowSize = (uint)Mathf.Ceil(entities.Count / (float)columnSize);

            // 左右方向のマージン
            // 画面奥側のエンティティは手前側よりもちょっと左右 (X軸方向) に移動させる
            const float Margin = 2.5f;

            // 台の大きさを取得 (仮)
            MeshRenderer fieldMesh = field.GetComponent<MeshRenderer>();

            // 配置領域のデータ (仮)
            var region = new
            {
                // 領域の幅：スクリーン幅
                // TODO: 今のところ Field の4分の1がいい塩梅になるが、将来的には画面サイズやエンティティの数に応じて変化させるべきかも
                Width = fieldMesh.bounds.size.x / 4f,

                // 領域の幅：台 (field) の奥行き
                Depth = fieldMesh.bounds.size.z
            };

            // 配置領域内でのローカル位置
            Vector3[,] localPositions = new Vector3[rowSize, columnSize];

            // 配置位置の計算
            // 配置領域（矩形）の原点は、左下を (0, 0) の基準とする
            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < columnSize; j++)
                {                    
                    localPositions[i, j] = new Vector3
                    {
                        // X座標は、配置方向とマージンを加味する
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
                // region の基準から配置されて行くので、X方向は region.Width / rowSize
                // Z方向は region.Depth / columnSize だけ余分な幅がある
                // そのため中心座標はそれらを引いてから計算する
                x = origin.x - direction * (region.Width - region.Width / rowSize) / 2f,
                y = origin.y,
                z = origin.z - (region.Depth - region.Depth / columnSize) / 2f
            };

            // 配置位置の適用
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

                    // 位置の変化量だけ移動させると、origin を中心とした配置になる
                    entities[index].transform.position = localPositions[i, j] + deltaCenter;
                }
            }

            if (!isDrawRegion)
            {
                return;
            }

            // 以降、配置領域の描画（デバッグ用）

            // 各エンティティの領域 = セル を描画するために、親を用意する
            var parent = Instantiate(new GameObject("Region"), this.transform);

            // セルの大きさを計算
            var cell = new
            {
                HalfWidth = region.Width / rowSize / 2f,
                HalfDepth = region.Depth / columnSize / 2f
            };

            // セルのメッシュ作成
            // この pos はセルの中心位置を示している
            foreach (var pos in entities.Select(entity => entity.transform.position))
            {
                // 四角形は 1.左下 2.左上 3.右下 4.右上 の順番
                Vector3[] vertices = new Vector3[]
                {
                    new Vector3(pos.x - cell.HalfWidth, pos.y, pos.z - cell.HalfDepth),
                    new Vector3(pos.x - cell.HalfWidth, pos.y, pos.z + cell.HalfDepth),
                    new Vector3(pos.x + cell.HalfWidth, pos.y, pos.z - cell.HalfDepth),
                    new Vector3(pos.x + cell.HalfWidth, pos.y, pos.z + cell.HalfDepth)
                };

                int[] triangles = new int[]
                {
                    0, 1, 2,
                    3, 2, 1
                };

                Mesh mesh = new()
                {
                    vertices = vertices,
                    triangles = triangles
                };

                mesh.RecalculateNormals();

                GameObject cellObject = new("Cell");
                cellObject.transform.SetParent(parent.transform);
                
                // マテリアルはまだ適用していない（仮なので）
                cellObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = cellObject.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
            }
        }
    }
}
