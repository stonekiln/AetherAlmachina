using VContainer;
using DIVFactor.Lifecycle;

namespace DIVFactor.Injectable
{
    /// <summary>
    /// 依存関係をInject可能なクラスを実装する
    /// </summary>
    public interface IInjectable
    {
        /// <summary>
        /// ここでフィールドのインジェクトを行う
        /// </summary>
        /// <param name="resolver">使用するcontainer</param>
        void Injection(InjectableResolver resolver);
    }
    /// <summary>
    /// インジェクトを行うためのコンテナ
    /// </summary>
    public class InjectableResolver
    {
        IObjectResolver Resolver { get; init; }
        readonly LifecycleEventHub usageEvents;
        public InjectableResolver(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        InjectableResolver(IObjectResolver resolver, LifecycleEventHub usageEvents)
        {
            Resolver = resolver;
            this.usageEvents = usageEvents;
        }

        internal InjectableResolver WithLifecycleEvents(LifecycleEventHub events)
            => new(Resolver, events);

        /// <summary>
        /// 引数にとった変数に依存関係を解決した状態のインスタンスを渡す
        /// </summary>
        /// <typeparam name="T">注入する型</typeparam>
        /// <param name="value">注入する変数</param>
        public void Inject<T>(out T value)
        {
            value = usageEvents != null && typeof(T) == typeof(LifecycleEventHub)
                ? (T)(object)usageEvents : Resolver.Resolve<T>();
        }
        /// <summary>
        /// DI登録された種類のインスタンスを取得する
        /// </summary>
        /// <typeparam name="T">取得するインスタンスの種類</typeparam>
        /// <returns>依存解決済みのインスタンス</returns>
        public T GetComponent<T>()
        {
            Inject(out T value);
            return value;
        }
    }
}
