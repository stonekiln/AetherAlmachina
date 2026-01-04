using System.IO;
using DConfig.CardLife;
using DConfig.PlayerLife.Event;
using DivFacter.Event;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.PlayerLife.Installer
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
    public class CardPrefabInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterFactory<SkillData, CardBase>(container => skilldata =>
            {
                GameObject cardLifetime = container.Instantiate(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardLifetime")));
                return cardLifetime.GetComponent<CardLifetimeScope>().Container.Resolve<CardBase>().Initialize(skilldata);
            }, Lifetime.Singleton);
        }
    }
}