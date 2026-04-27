using AetherAlmachina.Entities.Parameter;
using DConfig.EntityLife.Event;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// Entityの情報のみを参照するためのインターフェイス
    /// </summary>
    public interface IEntityInteraction
    {
        /// <summary>
        /// エンティティの現在地を表すインデックス
        /// </summary>
        public int SiblingIndex { get; }
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