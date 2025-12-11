using VContainer;

namespace DivFacter.Injectable
{
    /// <summary>
    /// 拡張メソッドだとoutを使用できないのでこのクラスでラップする
    /// </summary>
    public class InjectableResolver
    {
        readonly IObjectResolver Resolver;

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
    }
}