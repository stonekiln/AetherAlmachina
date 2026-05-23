using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Modifiers;
using DConfig.EntityLife.Event;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    public interface IStatusReader
    {
        /// <summary>
        /// 指定した種類のステータスの数値を取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>取得した数値</returns>
        float Get(StatusType type);
        /// <summary>
        /// 指定した種類のステータスの数値を整数値で取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>取得した数値</returns>
        int GetInt(StatusType type);
    }
    public interface IEnchantableStatus : IStatusReader
    {
        ResourceStateParameter Resource { get; }
        T GetModifiers<T>() where T : ModifierStock;
    }
    /// <summary>
    /// Entityの情報のみを参照するためのインターフェイス
    /// </summary>
    public interface IEntityInteraction
    {
        /// <summary>
        /// エンティティのレイアウト上の位置を表すインデックス
        /// </summary>
        Vector2Int LayoutIndex { get; }
        /// <summary>
        /// エンティティ間でターゲティングを行うためのイベント群
        /// </summary>
        TargetingEventBundle Targeting { get; }
        /// <summary>
        /// エンティティの行動を制御するためのイベント群
        /// </summary>
        ActionEventBundle Action { get; }
        /// <summary>
        /// エンティティの内部処理を制御するためのイベント群
        /// </summary>
        ProcessEventBundle Process { get; }
        /// <summary>
        /// エンティティのステータス
        /// </summary>
        IStatusReader Status { get; }
    }
    /// <summary>
    /// EntityのModifierを参照するためのインターフェイス
    /// </summary>
    public interface IEntityEnchantInteraction : IEntityInteraction
    {
        /// <summary>
        /// エンティティのステータス
        /// </summary>
        new IEnchantableStatus Status { get; }
    }
}
