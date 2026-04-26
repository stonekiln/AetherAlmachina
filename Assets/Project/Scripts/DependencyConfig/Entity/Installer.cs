using AetherAlmachina.Deck;
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

            builder.Register<DeckDrawEventBundle>(Lifetime.Singleton);
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
            builder.RegisterEvent<LockOnRequestEvent>();
            builder.RegisterEvent<LockOnResponseEvent>();
            builder.RegisterEvent<HitEvent>();

            builder.Register<LockOnEventBundle>(Lifetime.Singleton);
            builder.Register<TargetingEventBundle>(Lifetime.Singleton);
        }
    }
    public class CommandEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<SkillEndEvent>();
            builder.RegisterEvent<AttackEvent>();
            builder.RegisterEvent<DamageEvent>();
            builder.RegisterEvent<HealingEvent>();
            builder.RegisterEvent<OnHealedEvent>();

            builder.Register<CommandEventBundle>(Lifetime.Singleton);
        }
    }
}