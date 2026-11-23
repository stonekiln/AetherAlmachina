using System;
using System.Collections.Generic;
using DIVFactor.Lifecycle;
using VContainer;

namespace DIVFactor.Pooling
{
    /// <summary>コンテナを構築し直さず、ファクトリー引数だけを差し替える。</summary>
    public sealed class LifetimeArguments
    {
        readonly Dictionary<Type, IArgument> arguments = new();

        public static LifetimeArguments Create() => new();
        public static LifetimeArguments Create<T>(T value) => new LifetimeArguments().Add(value);
        public static LifetimeArguments Create<T1, T2>(T1 first, T2 second)
            => new LifetimeArguments().Add(first).Add(second);
        public static LifetimeArguments Create<T1, T2, T3>(T1 first, T2 second, T3 third)
            => new LifetimeArguments().Add(first).Add(second).Add(third);

        LifetimeArguments Add<T>(T value)
        {
            Type argumentType = typeof(T);
            if (argumentType == typeof(LifetimeArguments) || argumentType == typeof(LifecycleEventHub))
                throw new ArgumentException($"{argumentType} はDIVFactorの管理用の型です。");
            if (arguments.ContainsKey(argumentType))
                throw new ArgumentException($"ファクトリー引数の型 {argumentType} が重複しています。");
            arguments.Add(argumentType, new Argument<T>(value));
            return this;
        }

        public void Register(IContainerBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.RegisterInstance(this);
            foreach (IArgument argument in arguments.Values) argument.Register(builder);
        }

        public void Replace(LifetimeArguments next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (ReferenceEquals(this, next)) return;
            if (arguments.Count != next.arguments.Count)
                throw new ArgumentException("ファクトリー引数の組み合わせが異なります。", nameof(next));
            foreach (Type argumentType in arguments.Keys)
                if (!next.arguments.ContainsKey(argumentType))
                    throw new ArgumentException($"ファクトリー引数 {argumentType} がありません。", nameof(next));
            foreach (KeyValuePair<Type, IArgument> argument in arguments)
                argument.Value.CopyFrom(next.arguments[argument.Key]);
        }

        public void Clear()
        {
            foreach (IArgument argument in arguments.Values) argument.Clear();
        }

        interface IArgument
        {
            void Register(IContainerBuilder builder);
            void CopyFrom(IArgument source);
            void Clear();
        }

        sealed class Argument<T> : IArgument
        {
            T value;
            internal Argument(T value) => this.value = value;
            // アクセサーだけをTransientにする。固定のサービスのLifetimeは変更しない。
            public void Register(IContainerBuilder builder)
                => builder.Register<T>(_ => value, VContainer.Lifetime.Transient);
            public void CopyFrom(IArgument source) => value = ((Argument<T>)source).value;
            public void Clear() => value = default;
        }
    }
}
