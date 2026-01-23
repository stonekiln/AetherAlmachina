using UnityEngine;

namespace DIVFactor.Binder
{
    public interface IObjectBinder<T> where T : MonoBehaviour
    {
        public void Bind(T element);
    }
    public abstract class ObjectBinder<T> : MonoBehaviour, IObjectBinder<T> where T : MonoBehaviour
    {
        public void Bind(T element)
        {
            element.transform.SetParent(transform, false);
        }
    }
}