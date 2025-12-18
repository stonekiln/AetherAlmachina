using DivFacter.Injectable;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.CardLife
{
    public class CardLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CardBase>().UnderTransform(transform.parent);
            builder.RegisterComponentInHierarchy<CardSelecter>().UnderTransform(transform.parent);
            builder.RegisterComponentInHierarchy<CardDesign>().UnderTransform(transform.parent);
            builder.InjectMonoBehaviors(transform.parent.GetComponentsInChildren<MonoBehaviour>());
        }
    }
}