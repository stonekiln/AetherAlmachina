using UnityEngine;

namespace DIVFactor.Binder
{
    /// <summary>
    /// BinderInChildのジェネリック型を縛るためのインターフェイス
    /// </summary>
    public interface IObjectBinderBase { }
    /// <summary>
    /// このオブジェクトはBind可能である
    /// </summary>
    /// <typeparam name="T">バインドするMonoBehaviourの種類</typeparam>
    public interface IObjectBinder<T> : IObjectBinderBase where T : MonoBehaviour
    {
        /// <summary>
        /// 指定したMonoBehaviourを自身にバインドする
        /// </summary>
        /// <param name="element"></param>
        public void Bind(T element);
    }
    /// <summary>
    /// 指定したMonoBehaviourをバインドするMonoBehaviourを実装するためのクラス
    /// </summary>
    /// <typeparam name="T">バインドするMonoBehaviourの種類</typeparam>
    public abstract class ObjectBinder<T> : MonoBehaviour, IObjectBinder<T> where T : MonoBehaviour
    {
        public void Bind(T element)
        {
            element.transform.SetParent(transform, false);
        }
    }
}