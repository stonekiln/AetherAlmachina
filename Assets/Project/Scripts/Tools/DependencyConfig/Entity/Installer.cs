using DConfig.EntityLife.Event;
using DIVFactor.Event;
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
            builder.Register<EventBus<DeckGetEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<DeckDrawRequestEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<DeckDrawResponseEvent>>(Lifetime.Singleton);

            builder.Register<DeckDrawEventBundle>(Lifetime.Singleton);
        }
    }
    public class CardEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<CardSelectEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardCancelEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardInvokeEvent>>(Lifetime.Singleton);

            builder.Register<CardActivateEventBundle>(Lifetime.Singleton);
        }
    }
}