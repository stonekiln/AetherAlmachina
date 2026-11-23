using DIVFactor.Injectable;

namespace DIVFactor.Lifecycle
{
    // 個別の契約を維持し、クラス宣言でよく併用する組み合わせだけをまとめる。
    public interface IInitializableComponent : IInjectable, IInitializable { }
    public interface ISubscriberComponent : IInjectable, IUsageSubscriber { }

    public interface IReactiveComponent : IInitializableComponent, ISubscriberComponent { }
    public interface IActiveComponent : IReactiveComponent, IActivatable { }

    public interface IViewComponent : IInitializableComponent, IViewUpdatable { }
    public interface IReactiveView : IViewComponent, IUsageSubscriber { }
    public interface IActiveView : IViewComponent, IActivatable, IReleasable { }
}
