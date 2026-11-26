using DIVFactor.Lifecycle;
using R3;
using System;
using VContainer;

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
        readonly CompletionPhase injection;

        public Observable<Unit> Injected => injection?.Signal ?? Observable.Empty<Unit>();
        public void Fail(Exception exception) => injection?.Fail(exception);

        public InjectableResolver(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        InjectableResolver(IObjectResolver resolver, LifecycleEventHub usageEvents, CompletionPhase injection)
        {
            Resolver = resolver;
            this.usageEvents = usageEvents;
            this.injection = injection;
        }

        internal InjectableResolver WithLifecycleEvents(LifecycleEventHub events, CompletionPhase injection)
            => new(Resolver, events, injection);

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
