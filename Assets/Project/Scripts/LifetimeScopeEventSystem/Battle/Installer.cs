using LSES.Battle.Event;
using VContainer;
using VContainer.Unity;

namespace LSES.Battle.Installer
{
    public class CostEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<AutoIncreaseEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<BonusIncreaseEvent>>(Lifetime.Singleton);
        }
    }
    public class CardEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<CardSelectEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardCancelEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardInvokeEvent>>(Lifetime.Singleton);

            builder.Register<CardActiveEventBundle>(Lifetime.Singleton);
        }
    }
    public class DeckEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<DeckGetEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<DeckDrawRequestEvent>>(Lifetime.Singleton);
            
            builder.Register<DeckDrawEventBundle>(Lifetime.Singleton);
        }
    }
}