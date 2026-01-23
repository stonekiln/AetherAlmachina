using DIVFactor.Binder;
using DIVFactor.Event;
using R3;
using UnityEngine;
using VContainer;

namespace DIVFactor.Injectable
{
    /// <summary>
    /// 依存関係をInject可能なクラスを実装する
    /// </summary>
    public interface IInjectable
    {
        /// <summary>
        /// ここに注入する変数を宣言する
        /// </summary>
        /// <param name="resolver">使用するcontainer</param>
        public void InjectDependencies(InjectableResolver resolver);
    }
    /// <summary>
    /// 拡張メソッドだとoutを使用できないのでこのクラスでラップする
    /// </summary>
    public class InjectableResolver
    {
        IObjectResolver Resolver { get; init; }

        public InjectableResolver(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        /// <summary>
        /// 引数にとった変数に依存関係を解決した状態のインスタンスを渡す
        /// </summary>
        /// <typeparam name="T">注入する型</typeparam>
        /// <param name="value">注入する変数</param>
        public void Inject<T>(out T value)
        {
            value = Resolver.Resolve<T>();
        }
        public EventBus<T> GetEvent<T>() where T : EventObject
        {
            return Resolver.Resolve<EventBus<T>>();
        }
        /// <summary>
        /// DI登録されたTのインスタンスを返す
        /// </summary>
        /// <typeparam name="T">取得するインスタンスの種類</typeparam>
        /// <returns>依存解決済みのインスタンス</returns>
        public T GetComponent<T>()
        {
            return Resolver.Resolve<T>();
        }
        public void Bind<T>(T monoBehaviour) where T : MonoBehaviour
        {
            GetEvent<ActivateEvent>().Subscribe(_ => GetComponent<IObjectBinder<T>>().Bind(monoBehaviour)).AddTo(monoBehaviour);
        }
    }
}