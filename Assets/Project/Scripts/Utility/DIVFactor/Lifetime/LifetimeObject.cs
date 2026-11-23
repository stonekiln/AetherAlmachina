using System;
using System.Collections.Generic;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Pooling;
using DIVFactor.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime
{
    public abstract class LifetimeObject : LifetimeScope
    {
        readonly LifetimeOwnership ownership = new();
        readonly List<LifetimeObject> children = new();
        readonly List<LifetimePool> pools = new();
        LifetimeObject parentOwner;
        LifetimePool poolOwner;
        LifetimeArguments arguments;
        GameObject model;
        Transform modelParent;
        bool deferStart;
        bool closed;
        bool destroyScheduled;
        bool finalized;
        Action<IContainerBuilder> callback;
        LifecycleRunner lifecycle;
        public IReadOnlyList<GameObject> ChildObjects => ownership.Objects;
        internal bool IsInitialized => lifecycle?.State == LifecycleState.Active;
        internal bool IsUsable => !closed && Container != null && lifecycle != null;
        internal bool AcceptsSpawning => !closed && (lifecycle?.State == LifecycleState.Starting || lifecycle?.State == LifecycleState.Active);

        protected override void Awake()
        {
            LifetimeSeed seed = GetComponent<LifetimeSeed>();
            using (IDisposable parentOverride = seed != null && seed.Parent ? EnqueueParent(seed.Parent) : null)
            {
                if (seed != null)
                {
                    callback = seed.CallBack;
                    arguments = seed.Arguments;
                    poolOwner = seed.Pool;
                    deferStart = seed.DeferStart;
                    modelParent = seed.SpawnParent;
                    Destroy(seed);
                }
                arguments ??= LifetimeArguments.Create();
                base.Awake();
            }
        }

        protected override void OnDestroy()
        {
            destroyScheduled = true;
            finalized = true;
            try { CloseOwned(); }
            catch (Exception exception) { Debug.LogException(exception); }
            finally
            {
                try { base.OnDestroy(); }
                finally { ownership.Dispose(); NotifyDestroyed(); }
            }
        }

        protected abstract void Install(ContainerInstaller installer);
        protected abstract void Register(ComponentRegister register);

        protected sealed override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEventHub<LifecycleEventHub>();
            builder.RegisterInstance(ownership);
            arguments ??= LifetimeArguments.Create();
            arguments.Register(builder);
            Install(new(builder));
            Register(new(builder, transform));
            callback?.Invoke(builder);
            callback = null;
            builder.RegisterBuildCallback(BuildCallback);
        }

        void BuildCallback(IObjectResolver container)
        {
            MonoBehaviour[] components = GetComponentsInChildren<MonoBehaviour>(true);
            model = transform.GetChild(0).gameObject;
            ownership.Track(model);
            modelParent ??= transform.parent;
            model.transform.SetParent(modelParent, false);
            if (Parent) transform.SetParent(Parent.transform, false);
            parentOwner = Parent as LifetimeObject;
            parentOwner?.children.Add(this);
            foreach (MonoBehaviour component in components)
                if (component is ILifetimeSpawner spawner) spawner.SpawnConfigure(new(component.transform, this));

            lifecycle = new LifecycleRunner(components, container.Resolve<LifecycleEventHub>(),
                () => ownership.SetVisible(true), () => ownership.SetVisible(false), ScheduleDestroy,
                EndChildUses, poolOwner ? ReturnToPool : null, arguments.Clear);
            if (!deferStart) BeginUse(arguments);
        }

        internal void BeginUse(LifetimeArguments next)
        {
            if (!IsUsable) throw new ObjectDisposedException(GetType().Name);
            arguments.Replace(next);
            if (model && model.transform.parent != modelParent) model.transform.SetParent(modelParent, false);
            lifecycle.Run(new InjectableResolver(Container));
            if (!IsInitialized) throw new InvalidOperationException($"{GetType().Name} の使用開始が完了しませんでした。");
        }

        internal void TrackPool(LifetimePool pool)
        {
            if (closed) throw new ObjectDisposedException(GetType().Name);
            pools.Add(pool);
        }

        internal void UntrackPool(LifetimePool pool) => pools.Remove(pool);

        void ReturnToPool()
        {
            if (!poolOwner || !poolOwner.Return(this)) DestroyForOwner();
        }

        void EndChildUses()
        {
            List<Exception> failures = new();
            foreach (LifetimeObject child in children.ToArray())
            {
                if (!child || child.closed) continue;
                try { child.lifecycle?.EndForOwner(); }
                catch (Exception exception) { failures.Add(exception); }
            }
            if (failures.Count != 0) throw new AggregateException(failures);
        }

        void CloseOwned()
        {
            if (closed) return;
            closed = true;
            List<Exception> failures = new();
            try { lifecycle?.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }
            foreach (LifetimePool pool in pools.ToArray())
            {
                if (!pool) continue;
                try { pool.Dispose(); }
                catch (Exception exception) { failures.Add(exception); }
            }
            pools.Clear();
            foreach (LifetimeObject child in children.ToArray())
            {
                if (!child) continue;
                try { child.DestroyForOwner(); }
                catch (Exception exception) { failures.Add(exception); }
            }
            children.Clear();
            arguments?.Clear();
            if (failures.Count != 0) throw new AggregateException(failures);
        }

        internal void DestroyForOwner()
        {
            if (finalized) return;
            finalized = true;
            destroyScheduled = true;
            try { CloseOwned(); }
            finally
            {
                try { DisposeCore(); }
                finally { ownership.Dispose(); NotifyDestroyed(); Destroy(gameObject); }
            }
        }

        void ScheduleDestroy()
        {
            if (destroyScheduled) return;
            destroyScheduled = true;
            try { CloseOwned(); }
            finally { ownership.Dispose(); NotifyDestroyed(); Destroy(gameObject); }
        }

        void NotifyDestroyed()
        {
            callback = null;
            parentOwner?.children.Remove(this);
            if (poolOwner) poolOwner.NotifyDestroyed(this);
            poolOwner = null;
        }
    }
}
