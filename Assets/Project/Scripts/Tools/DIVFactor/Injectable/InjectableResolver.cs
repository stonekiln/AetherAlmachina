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
        /// ここでフィールドのインジェクトを行う
        /// </summary>
        /// <param name="resolver">使用するcontainer</param>
        public void Injection(InjectableResolver resolver);
    }
    /// <summary>
    /// インジェクトを行うためのコンテナ
    /// </summary>
    public class InjectableResolver
    {
        IObjectResolver Resolver { get; init; }
        EventPoint EventPoint { get; init; }

        public InjectableResolver(IObjectResolver resolver, EventPoint eventPoint)
        {
            Resolver = resolver;
            EventPoint = eventPoint;
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
        /// <summary>
        /// DI登録された種類のインスタンスを取得する
        /// </summary>
        /// <typeparam name="T">取得するインスタンスの種類</typeparam>
        /// <returns>依存解決済みのインスタンス</returns>
        public T GetComponent<T>()
        {
            return Resolver.Resolve<T>();
        }
        /// <summary>
        /// 指定したMonoBehaviourの種類のBinderにそのMonoBehaviourをバインドする
        /// </summary>
        /// <typeparam name="T">バインドするMonoBehaviourの種類</typeparam>
        /// <param name="monoBehaviour">バインドするMonoBehaviour</param>
        public void Bind<T>(T monoBehaviour) where T : MonoBehaviour
        {
            EventPoint.BindPoint.Subscribe(data =>
            {
                monoBehaviour.gameObject.SetActive(false);
                data.Lifetime.ChildObjects.Add(monoBehaviour.gameObject);
                GetComponent<IObjectBinder<T>>().Bind(monoBehaviour);
            });
        }
        /// <summary>
        /// ActivePointのObservableを返す
        /// </summary>
        /// <returns>ActivePointのObservable</returns>
        public Observable<ActiveEvent> ActivePointAsObservable()
        {
            return EventPoint.ActivePoint.AsObservable();
        }
        /// <summary>
        /// EndPointを発行する
        /// </summary>
        public void EntryEndPoint()
        {
            EventPoint.EndPoint.OnNext(new());
        }
    }
}