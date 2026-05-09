using System.Collections.Generic;
using DIVFactor.Injectable;
using DIVFactor.Event;
using AetherAlmachina.Entities;
using R3;
using UnityEngine;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// エンティティを配置するための基底クラス
    /// </summary>
    /// <typeparam name="TEvent">派生クラスで使用するイベント型</typeparam>
    public class EntityLayout<TEvent> : MonoBehaviour, IInjectable where TEvent : EventObject
    {
        /// <summary>
        /// ステージ設定
        /// </summary>
        protected StageSettingsAsset settings;
        /// <summary>
        /// エンティティを配置するためのイベントバス
        /// </summary>
        protected EventBus<TEvent> layoutEventBus;
        /// <summary>
        /// エンティティを配置する領域のサイズ
        /// </summary>
        [SerializeField] protected Vector3 regionSize = Vector3.one;

        public virtual void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out settings);
            resolver.Inject(out layoutEventBus);
        }

        void OnDrawGizmos()
        {
            // 配置領域をワイヤーフレームで表示
            Gizmos.DrawWireCube(transform.position, regionSize);
        }

        /// <summary>
        /// 複数のエンティティを origin を中心とした領域に、指定した列数 (columnSize) に揃えて配置する
        /// デフォルトで右方向に揃える
        /// </summary>
        /// <typeparam name="TEntity">エンティティ型</typeparam>
        /// <param name="entities">エンティティのリスト</param>
        /// <param name="origin">配置の中心点</param>
        /// <param name="layoutSize">列数と行数</param>
        /// <param name="reversed">
        /// X軸（Z軸の正方向が、<b>カメラ正面 = 画面奥</b> 側のときの、左右方向）における配置方向
        /// true の時は origin から見て右側に配置、false の時は左側に配置）
        /// </param>
        protected void Arrange<TEntity>(List<TEntity> entities, Vector3 origin, Vector2Int layoutSize, bool reversed)
            where TEntity : Entity
        {
            // 列数と行数に分解
            uint rowSize = (uint)layoutSize.x;
            uint columnSize = (uint)layoutSize.y;

            // 配置方向の係数
            int direction = reversed ? -1 : 1;

            // 左右方向のマージン
            // 画面奥側のエンティティは手前側よりもちょっと左右 (X軸方向) に移動させる
            const float Margin = 2.5f;

            // 配置領域内でのローカル位置
            // 配置領域の左下（配置方向が逆の時は右下）を (0, 0) とした座標系とする
            Vector3[,] localPositions = new Vector3[rowSize, columnSize];

            // ローカル位置から origin を中心とした位置への移動量
            // 変化後 - 変化前
            // 基準を 0 にして計算したものは変化量なし、直接指定
            Vector3 deltaCenter = new()
            {
                // ローカル位置では、origin を基準にした座標系と比べて、
                // X方向は regionSize.x / rowSize、Z方向は regionSize.z / columnSize だけ余分な幅がある
                // 更に origin を中心にするには余分な幅の半分だけずらさないといけない
                x = origin.x - direction * (regionSize.x - regionSize.x / rowSize) / 2f,
                y = origin.y,
                z = origin.z - (regionSize.z - regionSize.z / columnSize) / 2f
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

                    // ローカル位置の計算
                    localPositions[i, j] = new Vector3
                    {
                        // X座標は、配置方向とマージンを加味する
                        x = direction * (i / (float)rowSize * regionSize.x + j * Margin),
                        z = j / (float)columnSize * regionSize.z
                    };

                    // 配置位置の適用
                    // 位置の変化量だけ移動させると、origin を中心とした配置になる
                    entities[index].transform.position = localPositions[i, j] + deltaCenter;
                    entities[index].SetLayoutIndex(new Vector2Int(i, j));

                    // 自分を親にしてグループ化
                    entities[index].transform.SetParent(transform);
                }
            }
        }

        protected void Add<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            // TODO: 追加のときは、空いている場所に追加するようにする
        }

        // HACK: 多分使わないが一応残しておく
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
    }
}
