using DivFacter.Binder;
using UnityEngine;
using VContainer;

namespace DivFacter.Injectable
{
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
        /// <summary>
        /// DI登録されたTのインスタンスを返す
        /// </summary>
        /// <typeparam name="T">取得するインスタンスの種類</typeparam>
        /// <returns>依存解決済みのインスタンス</returns>
        public T GetComponent<T>()
        {
            return Resolver.Resolve<T>();
        }
        /// <summary>
        /// DI登録されたTの種類のBinderに自身を登録する
        /// </summary>
        /// <typeparam name="T">Binderの種類</typeparam>
        /// <param name="element">登録対象</param>
        public void RegisterBinder<T>(T element) where T : MonoBehaviour
        {
            Resolver.Resolve<IObjectBinder<T>>().Register(element);
        }
    }
}