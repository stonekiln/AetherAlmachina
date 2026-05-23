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
            builder.RegisterEvent<AttackEvent>();
            builder.RegisterEvent<DamageEvent>();
            builder.RegisterEvent<HealEvent>();
            builder.RegisterEvent<RecoveryEvent>();

            builder.RegisterEvent<SkillEndEvent>();

            builder.RegisterEvent<CostUpdateRequestEvent>();
            builder.RegisterEvent<CostUpdateResponseEvent>();
            builder.RegisterEvent<HPUpdateRequestEvent>();
            builder.RegisterEvent<HPUpdateResponseEvent>();
            builder.RegisterEvent<ShieldUpdateRequestEvent>();
            builder.RegisterEvent<ShieldUpdateResponseEvent>();
            builder.RegisterEvent<DisableUpdateRequestEvent>();
            builder.RegisterEvent<DisableUpdateResponseEvent>();

            builder.Register<ActionEventBundle>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventBundle<CostUpdateRequestEvent, CostUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventBundle<HPUpdateRequestEvent, HPUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventBundle<ShieldUpdateRequestEvent, ShieldUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventBundle<DisableUpdateRequestEvent, DisableUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventBundle>(Lifetime.Singleton);
            builder.Register<ProcessEventBundle>(Lifetime.Singleton);
        }
    }
}