using DConfig.StageLife.Event;
using DIVFactor.Extensions;
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
            builder.RegisterEvent<AutoIncreaseEvent>(Lifetime.Singleton);
            builder.RegisterEvent<BonusIncreaseEvent>(Lifetime.Singleton);
        }
    }
}