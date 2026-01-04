using System;
using DivFacter.Injectable;

namespace DivFacter.Binder
{
    public interface IObjectBinderBase
    {
        public void Reserve(InjectableResolver resolver);
    }
    public interface IObjectBinder<T> : IObjectBinderBase
    {
        public Type Key { get; }
        public void Register(T element);
    }
}