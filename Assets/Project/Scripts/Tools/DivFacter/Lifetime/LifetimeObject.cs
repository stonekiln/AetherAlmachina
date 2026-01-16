using System;
using DivFacter.Extensions;
using VContainer;
using VContainer.Unity;

namespace DivFacter.Lifetime
{
    public abstract class LifetimeObject : LifetimeScope
    {
        Action<IObjectResolver> Callback;
        protected override void Awake()
        {
            if (gameObject.TryGetComponent(out LifetimeSeed seed))
            {
                if (seed.Parent) EnqueueParent(seed.Parent);
                Callback = seed.CallBack;
                Destroy(seed);
            }
            base.Awake();
        }
        protected abstract void Install(InstallBuilder builder);
        protected abstract void Register(IContainerBuilder builder);

        protected sealed override void Configure(IContainerBuilder builder)
        {
            Install(new(builder));
            Register(builder);
            builder.ReserveInjection(transform.FindInjectable());
            builder.RegisterBuildCallback(container => Callback(container));
        }
    }
}