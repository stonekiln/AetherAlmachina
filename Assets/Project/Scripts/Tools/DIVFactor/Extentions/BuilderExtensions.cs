using DIVFactor.Event;
using VContainer;

namespace DIVFactor.Extensions
{
    static class BuilderExtensions
    {
        public static RegistrationBuilder RegisterEvent<TEvent>(this IContainerBuilder container, VContainer.Lifetime lifetime) where TEvent : EventObject
        {
            return container.Register<EventBus<TEvent>>(lifetime);
        }
        public static RegistrationBuilder RegisterEvent<TReq, TRes>(this IContainerBuilder container, VContainer.Lifetime lifetime)
        where TReq : EventObject
        where TRes : EventObject
        {
            container.Register<EventBus<TReq>>(lifetime);
            container.Register<EventBus<TRes>>(lifetime);
            return container.Register<EventChannel<TReq, TRes>>(lifetime);
        }
    }
}