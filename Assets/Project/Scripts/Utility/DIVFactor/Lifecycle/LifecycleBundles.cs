using DIVFactor.Injectable;

namespace DIVFactor.Lifecycle
{
    public interface ISubscriberComponent : IInjectable, IUsageSubscriber { }
    public interface IViewComponent : IInjectable, IViewUpdatable { }
    public interface IReactiveView : IViewComponent, IUsageSubscriber { }

    // 従来のバンドル名で実装するクラスにも、同じ完了通知の契約を提供する。
    public interface IInitializableComponent : IInjectable { }
    public interface IReactiveComponent : ISubscriberComponent { }
    public interface IActiveComponent : IReactiveComponent { }
    public interface IActiveView : IReactiveView, IReleasable { }
}
