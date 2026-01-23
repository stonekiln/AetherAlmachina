using System;
using System.Linq;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DIVFactor.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime
{
    public abstract class LifetimeObject : LifetimeScope
    {
        Action<IContainerBuilder> Callback;
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
        protected abstract void Install(ContainerInstaller installer);
        protected abstract void Register(ComponentRegister register);

        protected sealed override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(new EventBus<ActivateEvent>());
            Install(new(builder));
            Register(new(builder, transform));
            Callback(builder);
            builder.RegisterBuildCallback(container => BuildCallback(container));
        }
        void BuildCallback(IObjectResolver container)
        {
            MonoBehaviour[] monoBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (Component spawner in monoBehaviours.Where(mono => mono is ILifetimeSpawner))
            {
                ((ILifetimeSpawner)spawner).SpawnConfigure(new SpawnerBuilder(spawner.transform, this));
            }
            foreach (IInjectable injectable in monoBehaviours.OfType<IInjectable>())
            {
                injectable.InjectDependencies(new(container));
            }
            GameObject child = transform.GetChild(0).gameObject;
            child.transform.SetParent(transform.parent, false);
            transform.SetParent(Parent.transform, false);
            container.Resolve<EventBus<ActivateEvent>>().Publish(new());
            child.SetActive(true);
        }
    }
}