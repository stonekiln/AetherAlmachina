using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DIVFactor.Event;
using VContainer;

namespace DIVFactor.Extensions
{
    public static class BuilderExtensions
    {
        // 型の解析キャッシュと異なり、登録履歴は生成されるコンテナごとに独立させる。
        static readonly ConditionalWeakTable<IContainerBuilder, RegistrationState> Registrations = new();
        static readonly ConditionalWeakTable<IContainerBuilder, RegistrationState>.CreateValueCallback CreateState = _ => new();

        public static RegistrationBuilder RegisterEvent<TEvent>(this IContainerBuilder builder, VContainer.Lifetime lifetime = VContainer.Lifetime.Singleton) where TEvent : EventObject
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (!Enum.IsDefined(typeof(VContainer.Lifetime), lifetime))
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "未定義のLifetimeです。");

            RegistrationState registrations = Registrations.GetValue(builder, CreateState);
            lock (registrations)
            {
                Type eventPortType = typeof(EventPort<TEvent>);
                registrations.Validate(eventPortType, lifetime, eventPortType.ToString());
                return registrations.Register(builder, eventPortType, lifetime, eventPortType.ToString());
            }
        }

        /// <summary>
        /// Hub属性に従い配下のHubとEventPortを登録する。個別登録もRegisterEventに統一する。
        /// </summary>
        public static RegistrationBuilder RegisterEventHub<THub>(this IContainerBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            EventHubRegistrationPlan plan = EventHubRegistrationPlan.Get(typeof(THub));
            RegistrationState registrations = Registrations.GetValue(builder, CreateState);
            lock (registrations)
            {
                // 競合を全件検証してから追加し、エラー時に半分だけ登録されたHubを残さない。
                for (int index = 0; index < plan.Registrations.Count; index++)
                {
                    EventHubRegistrationPlan.Entry entry = plan.Registrations[index];
                    registrations.Validate(entry.ServiceType, entry.Lifetime, entry.Origin);
                }

                for (int index = 0; index < plan.Registrations.Count; index++)
                {
                    EventHubRegistrationPlan.Entry entry = plan.Registrations[index];
                    registrations.Register(builder, entry.ServiceType, entry.Lifetime, entry.Origin);
                }

                return registrations.Services[typeof(THub)].Builder;
            }
        }

        sealed class RegistrationState
        {
            internal Dictionary<Type, RegisteredService> Services { get; } = new();

            internal void Validate(Type serviceType, VContainer.Lifetime lifetime, string origin)
            {
                if (Services.TryGetValue(serviceType, out RegisteredService existing) && existing.Lifetime != lifetime)
                    throw new InvalidOperationException(
                        $"{serviceType} のLifetimeが競合しています: {existing.Lifetime} ({existing.Origin}) / {lifetime} ({origin})");
            }

            internal RegistrationBuilder Register(IContainerBuilder builder, Type serviceType, VContainer.Lifetime lifetime, string origin)
            {
                if (Services.TryGetValue(serviceType, out RegisteredService existing)) return existing.Builder;
                RegistrationBuilder registration = builder.Register(serviceType, lifetime);
                Services.Add(serviceType, new RegisteredService(registration, lifetime, origin));
                return registration;
            }
        }

        readonly struct RegisteredService
        {
            internal RegistrationBuilder Builder { get; }
            internal VContainer.Lifetime Lifetime { get; }
            internal string Origin { get; }

            internal RegisteredService(RegistrationBuilder builder, VContainer.Lifetime lifetime, string origin)
            {
                Builder = builder;
                Lifetime = lifetime;
                Origin = origin;
            }
        }
    }
}
