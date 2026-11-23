using System;
using System.Collections.Generic;
using System.Threading;

namespace DIVFactor.Lifecycle
{
    /// <summary>個体の寿命と分け、一回の使用に登録された処理を終了する。</summary>
    public sealed class UsageLifetime : IDisposable
    {
        readonly object gate = new();
        readonly List<IDisposable> resources = new();
        readonly CancellationTokenSource cancellation = new();
        readonly CancellationToken token;
        bool disposed;

        public CancellationToken Token => token;
        public bool IsDisposed { get { lock (gate) return disposed; } }

        public UsageLifetime() => token = cancellation.Token;

        internal void Add(IDisposable resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            lock (gate)
            {
                if (!disposed) { resources.Add(resource); return; }
            }
            resource.Dispose();
        }

        public void Dispose()
        {
            IDisposable[] pending;
            lock (gate)
            {
                if (disposed) return;
                disposed = true;
                pending = resources.ToArray();
                resources.Clear();
            }
            List<Exception> failures = new();
            // 一つの後始末の失敗で、ほかの購読を生かしたままにしない。
            foreach (IDisposable resource in pending)
            {
                try { resource.Dispose(); }
                catch (Exception exception) { failures.Add(exception); }
            }
            try { cancellation.Cancel(); }
            catch (Exception exception) { failures.Add(exception); }
            finally { cancellation.Dispose(); }
            if (failures.Count != 0) throw new AggregateException(failures);
        }
    }

    public static class UsageLifetimeExtensions
    {
        public static T AddTo<T>(this T resource, UsageLifetime lifetime) where T : IDisposable
        {
            if (lifetime == null) throw new ArgumentNullException(nameof(lifetime));
            lifetime.Add(resource);
            return resource;
        }
    }
}
