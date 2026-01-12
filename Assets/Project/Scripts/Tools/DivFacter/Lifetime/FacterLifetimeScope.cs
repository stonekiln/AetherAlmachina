using DivFacter.Lifetime;
using DivFacter.Extensions;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using System.Linq;

namespace DivFacter
{
    public abstract class FacterLifetimeScope : LifetimeScope
    {
        protected abstract void Install(InstallBuilder builder);
        protected abstract void Register(IContainerBuilder builder);

        protected sealed override void Configure(IContainerBuilder builder)
        {
            Install(new(builder));
            Register(builder);
            builder.ReserveInjection(transform.FindInjectable());
        }
    }
}
