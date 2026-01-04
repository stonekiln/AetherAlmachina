using DConfig.PlayerLife.Installer;
using DivFacter.EntryPoint;
using DivFacter.Event;
using DivFacter.Extensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.PlayerLife
{
    public class PlayerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CardEventInstaller().Install(builder);
            new DeckEventInstaller().Install(builder);
            new CardPrefabInstaller().Install(builder);

            builder.RegisterComponentInHierarchy<Player>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<HandVisualizer>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CostDisplay>().UnderTransform(transform);
            builder.RegisterBinderInHierarchy<CardBinder>();

            builder.ReserveInjection(transform.FindInjectable());
            builder.ReserveBinding(Parent.transform.FindBinder());
        }
    }
}
