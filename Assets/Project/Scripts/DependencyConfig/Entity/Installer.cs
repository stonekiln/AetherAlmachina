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

            builder.Register<DeckDrawEventHub>(Lifetime.Singleton);
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

            builder.Register<CardActiveEventHub>(Lifetime.Singleton);
        }
    }
    public class CommandEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<SkillEndEvent>();

            builder.RegisterEvent<LockOnRequestEvent>();
            builder.RegisterEvent<LockOnResponseEvent>();
            builder.Register<LockOnEventHub>(Lifetime.Singleton);

            builder.RegisterEvent<CostUpdateRequestEvent>();
            builder.RegisterEvent<CostUpdateResponseEvent>();
            builder.RegisterEvent<HPUpdateRequestEvent>();
            builder.RegisterEvent<HPUpdateResponseEvent>();
            builder.RegisterEvent<ShieldUpdateRequestEvent>();
            builder.RegisterEvent<ShieldUpdateResponseEvent>();
            builder.RegisterEvent<DisableUpdateRequestEvent>();
            builder.RegisterEvent<DisableUpdateResponseEvent>();

            builder.Register<ResourceUpdateEventHub<CostUpdateRequestEvent, CostUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventHub<HPUpdateRequestEvent, HPUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventHub<ShieldUpdateRequestEvent, ShieldUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventHub<DisableUpdateRequestEvent, DisableUpdateResponseEvent>>(Lifetime.Singleton);
            builder.Register<ResourceUpdateEventHub>(Lifetime.Singleton);

            builder.Register<InteractionEventHub>(Lifetime.Singleton);
        }
    }
}