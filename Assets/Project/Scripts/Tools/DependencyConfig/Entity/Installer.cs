using DConfig.EntityLife.Event;
using DIVFactor.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.EntityLife.Installer
{
    /// <summary>
    /// デッキに関するイベントのDI登録
    /// </summary>
    public class DeckEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<DeckGetEvent>();
            builder.RegisterEvent<DeckDrawRequestEvent>();
            builder.RegisterEvent<DeckDrawResponseEvent>();

            builder.Register<DeckDrawEvent>(Lifetime.Singleton);
            builder.Register<DeckController>(Lifetime.Singleton);
        }
    }
    public class CardEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<CardSelectEvent>();
            builder.RegisterEvent<CardCancelEvent>();
            builder.RegisterEvent<CardInvokeEvent>();

            builder.Register<CardActiveEventBundle>(Lifetime.Singleton);
        }
    }
    public class TargetingEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<SkillActiveEvent>();
            builder.RegisterEvent<TargetingEvent>();
            builder.RegisterEvent<HitEvent>();

            builder.Register<AttackEventBundle>(Lifetime.Singleton);
        }
    }
}