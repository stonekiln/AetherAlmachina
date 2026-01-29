using System;
using R3;

namespace DIVFactor.Event
{
    /// <summary>
    /// 全てのイベントメッセージはこれを継承すること
    /// </summary>
    public abstract record EventObject;
    /// <summary>
    /// イベントを発行するためのクラス
    /// </summary>
    /// <typeparam name="T">イベントメッセージ</typeparam>
    public class EventBus<T> : Subject<T> where T : EventObject { }
    public class EventChannel<TReq, TRes>
    where TReq : EventObject
    where TRes : EventObject
    {
        EventBus<TReq> RequestEvent { get; init; }
        EventBus<TRes> ResponseEvent { get; init; }
        public EventChannel(EventBus<TReq> request, EventBus<TRes> response)
        {
            RequestEvent = request;
            ResponseEvent = response;
        }
        public void Call(TReq req) => RequestEvent.OnNext(req);
        public IDisposable Response(Action<TRes> response) => ResponseEvent.Subscribe(response);
        public IDisposable Reply(Func<TReq, TRes> reply) => RequestEvent.Subscribe(request => ResponseEvent.OnNext(reply(request)));
    }
}