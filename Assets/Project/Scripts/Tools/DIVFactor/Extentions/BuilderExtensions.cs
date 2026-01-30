using DIVFactor.Event;
using VContainer;

namespace DIVFactor.Extensions
{
    static class BuilderExtensions
    {
        public static RegistrationBuilder RegisterEvent<TEvent>(this IContainerBuilder container, VContainer.Lifetime lifetime = VContainer.Lifetime.Singleton) where TEvent : EventObject
        {
            return container.Register<EventBus<TEvent>>(lifetime);
        }
    }
}