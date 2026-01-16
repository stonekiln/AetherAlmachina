using DivFacter.Event;

namespace DConfig.StageLife.Event
{
    /// <summary>
    /// コストの自動回復を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Delta">回復量</param>
    public record AutoIncreaseEvent(int Delta) : EventObject;
    /// <summary>
    /// コストの回復を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Delta">回復量</param>
    public record BonusIncreaseEvent(int Delta) : EventObject;
}