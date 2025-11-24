using System;
using R3;

namespace LSES
{
    public class EventBus<T> where T : EventObject
    {
        readonly Subject<T> Event = new();
        public IDisposable Subscribe(Action<T> action) => Event.Subscribe(action);
        public void Publish(T value) => Event.OnNext(value);
    }

    public abstract record EventObject;
}