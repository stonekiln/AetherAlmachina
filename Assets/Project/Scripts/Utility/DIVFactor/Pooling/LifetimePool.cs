using DIVFactor.Lifetime;
using System;
using UnityEngine;
using VContainer;

namespace DIVFactor.Pooling
{
    /// <summary>スポナーに自動付与され、同じ親スコープ・同じファクトリーの個体を保持する。</summary>
    public sealed class LifetimePool : MonoBehaviour, IDisposable
    {
        LifetimePoolStorage<LifetimeObject> storage;
        LifetimeObject owner;
        bool disposed;
        public int ActiveCount => storage?.ActiveCount ?? 0;
        public int InactiveCount => storage?.InactiveCount ?? 0;

        internal void Configure(LifetimeObject owner, Func<LifetimeArguments, LifetimeObject> create, int maxRetained)
        {
            if (storage != null || disposed) throw new InvalidOperationException("プールは設定済みです。");
            this.owner = owner ? owner : throw new ArgumentNullException(nameof(owner));
            storage = new LifetimePoolStorage<LifetimeObject>(create,
                (scope, arguments) => scope.BeginUse(arguments), scope => scope.DestroyForOwner(),
                scope => scope && scope.IsUsable, maxRetained);
            owner.TrackPool(this);
        }

        internal IObjectResolver Rent(LifetimeArguments arguments)
        {
            if (disposed || !owner || !owner.AcceptsSpawning)
                throw new InvalidOperationException("所有元のLifetimeが終了しているため、取得できません。");
            return storage.Rent(arguments).Container;
        }

        internal bool Return(LifetimeObject scope)
            => !disposed && owner && owner.IsUsable && storage.Return(scope);

        internal void NotifyDestroyed(LifetimeObject scope) => storage?.NotifyDestroyed(scope);

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try { storage?.Dispose(); }
            finally { if (owner) owner.UntrackPool(this); }
        }

        void OnDestroy()
        {
            try { Dispose(); }
            catch (Exception exception) { Debug.LogException(exception); }
        }
    }
}
