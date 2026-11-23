using System;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DIVFactor.Lifetime.Root
{
    public class RootLifetimeScope : LifetimeScope
    {
        LifecycleRunner lifecycle;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEventHub<LifecycleEventHub>();
            FindSpawner();
            builder.RegisterBuildCallback(container =>
            {
                lifecycle = new LifecycleRunner(Array.Empty<object>(), container.Resolve<LifecycleEventHub>(),
                    null, null, () => Destroy(gameObject), CloseChildren);
                lifecycle.Run(new InjectableResolver(container));
            });
        }

        protected override void OnDestroy()
        {
            try { lifecycle?.Dispose(); }
            catch (Exception exception) { Debug.LogException(exception); }
            finally { base.OnDestroy(); }
        }

        void FindSpawner()
        {
            foreach (Component component in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
                if (component is ILifetimeSpawner spawner) spawner.SpawnConfigure(new(component.transform, null));
        }

        void CloseChildren()
        {
            // シーンのRootには通常のMonoBehaviourを自動注入せず、生成済みの子スコープだけ終了する。
            foreach (LifetimeObject child in GetComponentsInChildren<LifetimeObject>(true))
            {
                if (!child) continue;
                try { child.DestroyForOwner(); }
                catch (Exception exception) { Debug.LogException(exception); }
            }
        }
    }
}
