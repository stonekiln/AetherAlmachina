using DivFacter.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.CardLife
{
    public class CardLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CardBase>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CardSelector>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CardDesign>().UnderTransform(transform);
            
            builder.ReserveInjection(transform.FindInjectable());
            builder.ReserveBinding(Parent.Parent.transform.FindBinder());
        }
    }
}