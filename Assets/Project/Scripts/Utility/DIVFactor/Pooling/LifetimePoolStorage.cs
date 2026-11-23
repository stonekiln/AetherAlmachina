using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace DIVFactor.Pooling
{
    /// <summary>Unityのプールを使用し、貸出中と待機中の両方の個体を所有する。</summary>
    public sealed class LifetimePoolStorage<T> : IDisposable where T : class
    {
        readonly Func<LifetimeArguments, T> create;
        readonly Action<T, LifetimeArguments> begin;
        readonly Action<T> destroy;
        readonly Func<T, bool> isAlive;
        readonly HashSet<Entry> entries = new();
        readonly Dictionary<T, Entry> instances = new(IdentityComparer.Instance);
        readonly ObjectPool<Entry> pool;
        bool disposed;

        public int ActiveCount => entries.Count(entry => entry.Rented && Alive(entry.Instance));
        public int InactiveCount => entries.Count(entry => !entry.Rented && Alive(entry.Instance));
        public bool IsDisposed => disposed;

        public LifetimePoolStorage(Func<LifetimeArguments, T> create, Action<T, LifetimeArguments> begin,
            Action<T> destroy, Func<T, bool> isAlive, int maxRetained = 64)
        {
            this.create = create ?? throw new ArgumentNullException(nameof(create));
            this.begin = begin ?? throw new ArgumentNullException(nameof(begin));
            this.destroy = destroy ?? throw new ArgumentNullException(nameof(destroy));
            this.isAlive = isAlive ?? throw new ArgumentNullException(nameof(isAlive));
            if (maxRetained <= 0) throw new ArgumentOutOfRangeException(nameof(maxRetained));
            pool = new ObjectPool<Entry>(CreateEntry, null, null, DestroyEntry, true, 0, maxRetained);
        }

        public T Rent(LifetimeArguments arguments)
        {
            if (disposed) throw new ObjectDisposedException(nameof(LifetimePoolStorage<T>));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            Entry entry = pool.Get();
            entry.Rented = true;
            try
            {
                if (!Alive(entry.Instance))
                {
                    RemoveInstance(entry);
                    T instance = create(arguments) ?? throw new InvalidOperationException("個体の生成に失敗しました。");
                    if (disposed)
                    {
                        destroy(instance);
                        throw new ObjectDisposedException(nameof(LifetimePoolStorage<T>));
                    }
                    entry.Instance = instance;
                    instances.Add(instance, entry);
                }
                begin(entry.Instance, arguments);
                if (disposed || !entry.Rented || !Alive(entry.Instance))
                    throw new InvalidOperationException("使用開始処理中に個体が終了しました。");
                return entry.Instance;
            }
            catch (Exception failure)
            {
                // 開始中に自分で返却した個体を二重に返却しない。
                if (!disposed && entries.Contains(entry) && entry.Rented)
                {
                    try { DestroyInstance(entry); }
                    catch (Exception cleanup) { failure = new AggregateException(failure, cleanup); }
                    finally
                    {
                        entry.Rented = false;
                        pool.Release(entry);
                    }
                }
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
                throw;
            }
        }

        public bool Return(T instance)
        {
            if (disposed || ReferenceEquals(instance, null) || !instances.TryGetValue(instance, out Entry entry) || !entry.Rented)
                return false;
            entry.Rented = false;
            pool.Release(entry);
            return true;
        }

        public void NotifyDestroyed(T instance)
        {
            if (ReferenceEquals(instance, null) || !instances.TryGetValue(instance, out Entry entry)) return;
            RemoveInstance(entry);
            if (entry.Rented)
            {
                entry.Rented = false;
                if (!disposed) pool.Release(entry);
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            List<Exception> failures = new();
            foreach (Entry entry in entries.ToArray())
            {
                try { DestroyInstance(entry); }
                catch (Exception exception) { failures.Add(exception); }
            }
            pool.Clear();
            entries.Clear();
            instances.Clear();
            if (failures.Count != 0) throw new AggregateException(failures);
        }

        bool Alive(T instance) => !ReferenceEquals(instance, null) && isAlive(instance);
        Entry CreateEntry()
        {
            Entry entry = new();
            entries.Add(entry);
            return entry;
        }
        void RemoveInstance(Entry entry)
        {
            if (!ReferenceEquals(entry.Instance, null)) instances.Remove(entry.Instance);
            entry.Instance = null;
        }
        void DestroyInstance(Entry entry)
        {
            T instance = entry.Instance;
            RemoveInstance(entry);
            if (Alive(instance)) destroy(instance);
        }
        void DestroyEntry(Entry entry)
        {
            entries.Remove(entry);
            DestroyInstance(entry);
        }

        sealed class Entry { internal T Instance; internal bool Rented; }
        sealed class IdentityComparer : IEqualityComparer<T>
        {
            internal static readonly IdentityComparer Instance = new();
            public bool Equals(T first, T second) => ReferenceEquals(first, second);
            public int GetHashCode(T instance) => RuntimeHelpers.GetHashCode(instance);
        }
    }
}
