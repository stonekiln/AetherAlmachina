using DIVFactor.Injectable;

namespace DIVFactor.Lifecycle
{
    public interface IViewUpdatable { void UpdateView(ViewContext context); }
    public interface IUsageSubscriber { void Subscribe(SubscriptionContext context); }
    public interface IReleasable { void Release(); }

    public interface ISubscriberComponent : IInjectable, IUsageSubscriber { }
    public interface IViewUpdater : IInjectable, IViewUpdatable { }
    public interface ISubscribingViewUpdater : IViewUpdater, ISubscriberComponent { }
}
