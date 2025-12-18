namespace DivFacter.Injectable
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
}