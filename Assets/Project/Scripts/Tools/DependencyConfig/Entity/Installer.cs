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
            builder.RegisterEvent<DeckGetEvent>(Lifetime.Singleton);
            builder.RegisterEvent<DeckDrawRequestEvent, DeckDrawResponseEvent>(Lifetime.Singleton);
            builder.Register<DeckController>(Lifetime.Singleton);
        }
    }
    public class CardEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<CardSelectEvent>(Lifetime.Singleton);
            builder.RegisterEvent<CardCancelEvent>(Lifetime.Singleton);
            builder.RegisterEvent<CardInvokeEvent>(Lifetime.Singleton);

            builder.Register<CardActivateEventBundle>(Lifetime.Singleton);
        }
    }
}