using AetherAlmachina.Entities.Parameter;
using DConfig.EntityLife.Event;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// Entityの情報のみを参照するためのインターフェイス
    /// </summary>
    public interface IEntityInteraction
    {
        /// <summary>
        /// エンティティのレイアウト上の位置を表すインデックス
        /// </summary>
        public Vector2Int LayoutIndex { get; }
        /// <summary>
        /// エンティティ間でターゲティングを行うためのイベント群
        /// </summary>
        public TargetingEventBundle Targeting { get; }
        /// <summary>
        /// エンティティの行動を制御するためのイベント群
        /// </summary>
        public ActionEventBundle Action { get; }
        /// <summary>
        /// エンティティの内部処理を制御するためのイベント群
        /// </summary>
        public ProcessEventBundle Process { get; }
        /// <summary>
        /// エンティティのステータス
        /// </summary>
        public StatusParameter Status { get; }
    }
}
