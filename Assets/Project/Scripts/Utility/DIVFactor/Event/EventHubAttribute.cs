using System;

namespace DIVFactor.Event
{
    /// <summary>
    /// Hub自身と直下のEventPortのLifetimeを指定する。子Hubは自身の指定を使う。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EventHubAttribute : Attribute
    {
        public VContainer.Lifetime Lifetime { get; }

        public EventHubAttribute(VContainer.Lifetime lifetime = VContainer.Lifetime.Singleton)
        {
            Lifetime = lifetime;
        }
    }
}