using AetherAlmachina.Entities.Parameter;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// Entityの情報のみを参照するためのインターフェイス
    /// </summary>
    public interface IEntityInteraction
    {
        string Name { get; }
        /// <summary>
        /// エンティティのレイアウト上の位置を表すインデックス
        /// </summary>
        Vector2Int LayoutIndex { get; }
        /// <summary>
        /// エンティティのステータス
        /// </summary>
        StatusParameter Status { get; }
        /// <summary>
        /// エンティティの内部処理を制御するためのイベント群
        /// </summary>
        InteractionEventBundle Interaction { get; }
    }
}
