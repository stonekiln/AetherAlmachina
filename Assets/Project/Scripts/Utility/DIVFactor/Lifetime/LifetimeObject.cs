using System;
using System.Collections.Generic;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Pooling;
using DIVFactor.Spawner;
using DIVFactor.Mold;
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
        MoldMetadata metadata;
        PrefabCatalog catalog;
        bool sceneRoot;
        bool prepared;
        internal PrefabCatalog Catalog => catalog;
        bool closed;
        bool destroyScheduled;
        bool finalized;
        LifecycleRunner lifecycle;
        public IReadOnlyList<GameObject> ChildObjects => ownership.Objects;
        internal bool IsInitialized => lifecycle?.State == LifecycleState.Active;
        internal bool IsUsable => !closed && Container != null && lifecycle != null;
        internal bool AcceptsSpawning => !closed && (lifecycle?.State == LifecycleState.Starting || lifecycle?.State == LifecycleState.Active);

        protected sealed override void Awake()
        {
            autoRun = false;
            base.Awake();
        }

        internal void Prepare(LifetimeObject parent, MoldMetadata metadata, PrefabCatalog catalog,
            LifetimeArguments arguments, LifetimePool pool, Transform displayParent, bool sceneRoot = false)
        {
            if (prepared || Container != null) throw new InvalidOperationException("Lifetimeは設定済みです。");
            if (!metadata || !metadata.IsBaked) throw new InvalidOperationException("Moldの事前生成情報がないか旧形式です。Editorで DIVFactor/Rebuild open scenes を実行してください。");
            this.metadata = metadata;
            this.catalog = catalog;
            this.arguments = arguments ?? LifetimeArguments.Create();
            this.sceneRoot = sceneRoot;
            poolOwner = pool;
            modelParent = displayParent;
            parentReference.Object = parent;
            if (!sceneRoot)
            {
                model = metadata.gameObject;
                ownership.Track(model);
            }
            prepared = true;
        }

        internal void BuildPrepared()
        {
            if (!prepared || Container != null) throw new InvalidOperationException("初期設定前または構築済みです。");
            if (parentReference.Object && parentReference.Object.Container == null)
                throw new InvalidOperationException("親コンテナを先に構築してください。");
            using (EnqueueParent(parentReference.Object)) Build();
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
            if (!prepared) throw new InvalidOperationException("Prepareの前に構築できません。");
            builder.RegisterEventHub<LifecycleEventHub>();
            builder.RegisterInstance(ownership);
            arguments ??= LifetimeArguments.Create();
            arguments.Register(builder);
            Install(new(builder));
            Register(new(builder, metadata));
            builder.RegisterBuildCallback(BuildCallback);
        }

        void BuildCallback(IObjectResolver container)
        {
            if (!prepared) throw new InvalidOperationException("Prepareの前に構築できません。");
            if (model) model.transform.SetParent(modelParent, false);
            if (Parent) transform.SetParent(Parent.transform, false);
            parentOwner = Parent as LifetimeObject;
            parentOwner?.children.Add(this);
            foreach (MonoBehaviour component in metadata.Spawners)
                if (component) ((ILifetimeSpawner)component).SpawnConfigure(new(component, this, metadata));

            lifecycle = new LifecycleRunner(sceneRoot ? LifecycleParticipants.From(Array.Empty<object>()) : metadata.Participants, container.Resolve<LifecycleEventHub>(),
                () => ownership.SetVisible(true), () => ownership.SetVisible(false), ScheduleDestroy,
                EndChildUses, poolOwner ? ReturnToPool : null, arguments.Clear);

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
            parentOwner?.children.Remove(this);
            if (poolOwner) poolOwner.NotifyDestroyed(this);
            poolOwner = null;
        }
    }
}
