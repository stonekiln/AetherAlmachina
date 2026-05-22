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
            builder.RegisterEvent<AutoIncreaseEvent>();
            builder.RegisterEvent<BonusIncreaseEvent>();
        }
    }
    /// <summary>
    /// 行動ゲージに関するイベントのDI登録
    /// </summary>
    public class ActGaugeInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<SkillActivateEvent>();
        }
    }
    /// <summary>
    /// レイアウトに関するイベントのDI登録
    /// </summary>
    public class LayoutEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<FriendlyLayoutEvent>();
            builder.RegisterEvent<HostileLayoutEvent>();
            builder.RegisterEvent<LayoutIndexEvent>();
        }
    }
}
