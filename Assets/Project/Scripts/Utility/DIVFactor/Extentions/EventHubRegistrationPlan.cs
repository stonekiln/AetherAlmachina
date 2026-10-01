using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DIVFactor.Event;

namespace DIVFactor.Extensions
{
    sealed class EventHubRegistrationPlan
    {
        static readonly ConcurrentDictionary<Type, Lazy<EventHubRegistrationPlan>> Cache = new();
        static readonly Func<Type, Lazy<EventHubRegistrationPlan>> PlanFactory = CreatePlan;
        internal IReadOnlyList<Entry> Registrations { get; }

        EventHubRegistrationPlan(Type hubType)
        {
            List<Entry> registrations = new();
            Dictionary<Type, Entry> registeredTypes = new();
            HashSet<Type> completedHubs = new();
            List<Type> hubPath = new();
            Visit(hubType);
            Registrations = registrations.AsReadOnly();

            void Visit(Type currentHub)
            {
                if (hubPath.Contains(currentHub))
                    throw new InvalidOperationException($"Hubが循環参照しています: {DescribePath(currentHub)}");
                if (completedHubs.Contains(currentHub)) return;

                // 型引数が毎回変わる再帰でも無制限に型情報を生成しない。
                if (hubPath.Count >= 8)
                    throw new InvalidOperationException($"Hubの階層が8段を超えています: {DescribePath(currentHub)}");

                hubPath.Add(currentHub);
                EventHubMetadata metadata = EventHubMetadata.Get(currentHub);
                foreach (Type dependencyType in metadata.Dependencies)
                {
                    if (EventHubMetadata.IsEventPort(dependencyType))
                        Add(dependencyType, metadata.Lifetime);
                    else
                        Visit(dependencyType);
                }
                Add(currentHub, metadata.Lifetime);
                hubPath.RemoveAt(hubPath.Count - 1);
                completedHubs.Add(currentHub);
            }

            void Add(Type serviceType, VContainer.Lifetime lifetime)
            {
                if (registeredTypes.TryGetValue(serviceType, out Entry existing))
                {
                    if (existing.Lifetime != lifetime)
                        throw new InvalidOperationException(
                            $"{serviceType} のLifetimeが競合しています: {existing.Lifetime} ({existing.Origin}) / {lifetime} ({DescribePath(serviceType)})");
                    return;
                }

                Entry registration = new(serviceType, lifetime, DescribePath(serviceType));
                registeredTypes.Add(serviceType, registration);
                registrations.Add(registration);
            }

            string DescribePath(Type serviceType)
            {
                string currentPath = string.Join(" -> ", hubPath);
                return hubPath.Count > 0 && hubPath[hubPath.Count - 1] == serviceType
                    ? currentPath
                    : $"{currentPath} -> {serviceType}";
            }
        }

        internal static EventHubRegistrationPlan Get(Type hubType) => Cache.GetOrAdd(hubType, PlanFactory).Value;

        static Lazy<EventHubRegistrationPlan> CreatePlan(Type hubType) => new(() => new EventHubRegistrationPlan(hubType), LazyThreadSafetyMode.ExecutionAndPublication);

        internal readonly struct Entry
        {
            internal Type ServiceType { get; }
            internal VContainer.Lifetime Lifetime { get; }
            internal string Origin { get; }

            internal Entry(Type serviceType, VContainer.Lifetime lifetime, string origin)
            {
                ServiceType = serviceType;
                Lifetime = lifetime;
                Origin = origin;
            }
        }
    }
}
