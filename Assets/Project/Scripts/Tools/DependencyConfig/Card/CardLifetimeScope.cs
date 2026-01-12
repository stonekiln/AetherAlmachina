using System.IO;
using DivFacter;
using DivFacter.Extensions;
using DivFacter.Lifetime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.CardLife
{
    public class CardLifetimeScope : FacterLifetimeScope
    {
        protected override void Install(InstallBuilder builder)
        {
            
        }

        protected override void Register(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CardBase>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CardSelector>().UnderTransform(transform);
            builder.RegisterComponentInHierarchy<CardDesign>().UnderTransform(transform);

            //builder.ReserveBinding(Parent.Parent.transform.FindBinder());
        }
    }
}