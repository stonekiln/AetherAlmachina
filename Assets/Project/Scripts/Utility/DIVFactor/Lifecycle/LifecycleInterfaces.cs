namespace DIVFactor.Lifecycle
{
    public interface IInitializable { void Initialize(); }
    public interface IViewUpdatable { void UpdateView(); }
    public interface IUsageSubscriber { void Subscribe(UsageLifetime lifetime); }
    public interface IActivatable { void Activate(); }
    public interface IReleasable { void Release(); }
}
