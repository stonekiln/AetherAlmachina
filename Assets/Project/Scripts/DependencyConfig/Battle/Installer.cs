using System.IO;
using DConfig.Battle.Event;
using DivFacter.Event;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.Battle.Installer
{
    /// <summary>
    /// コストに関するイベントのDI登録
    /// </summary>
    public class CostEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<AutoIncreaseEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<BonusIncreaseEvent>>(Lifetime.Singleton);
        }
    }
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
            builder.Register<DeckEventBundle>(Lifetime.Singleton);
        }
    }
    public class CardEventInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<EventBus<CardSelectEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardCancelEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardInvokeEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardCreateRequestEvent>>(Lifetime.Singleton);
            builder.Register<EventBus<CardCreateResponseEvent>>(Lifetime.Singleton);

            builder.Register<CardCreateEventBundle>(Lifetime.Singleton);
            builder.Register<CardActivateEventBundle>(Lifetime.Singleton);
        }
    }
    public class CardPrefabInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            GameObject prefab = Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase"));
            builder.RegisterComponentInNewPrefab(prefab.GetComponent<CardBase>(), Lifetime.Singleton);
            builder.UseComponents(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")).transform, builder =>
            {
                builder.AddInHierarchy<CardDesign>();
                builder.AddInHierarchy<CardSelecter>();
            });
            builder.RegisterFactory<SkillData, GameObject>(container => skilldata =>
            {
                GameObject CreatedCard = container.Instantiate(prefab);
                CardBase cardBase = CreatedCard.GetComponent<CardBase>();
                cardBase.SetSkilldata(skilldata);
                container.InjectGameObject(prefab);
                cardBase.Design.Initialize(cardBase);
                cardBase.Selecter.Initialize(cardBase);
                return CreatedCard;
            }, Lifetime.Singleton);
        }
    }
}