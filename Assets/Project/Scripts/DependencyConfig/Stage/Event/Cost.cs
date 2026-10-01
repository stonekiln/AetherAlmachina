using DIVFactor.Event;
using DIVFactor.Extensions;
using VContainer;
using VContainer.Unity;

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

    /// <summary>
    /// コストに関するイベントのDI登録
    /// </summary>
    public class CostEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<AutoIncreaseEvent>();
            builder.RegisterEvent<BonusIncreaseEvent>();
        }
    }
}