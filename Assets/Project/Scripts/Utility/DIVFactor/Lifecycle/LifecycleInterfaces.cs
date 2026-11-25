namespace DIVFactor.Lifecycle
{
    public interface IViewUpdatable { void UpdateView(ViewContext context); }
    public interface IUsageSubscriber { void Subscribe(SubscriptionContext context); }
    public interface IReleasable { void Release(); }
}
