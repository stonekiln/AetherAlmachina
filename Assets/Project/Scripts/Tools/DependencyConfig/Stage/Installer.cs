using DConfig.StageLife.Event;
using DivFacter.Event;
using VContainer;
using VContainer.Unity;

namespace DConfig.StageLife.Installer
{
    /// <summary>
    /// コストに関するイベントのDI登録
    /// </summary>
    public class CostEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<AutoIncreaseEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<BonusIncreaseEvent>>(Lifetime.Singleton);
        }
    }
}