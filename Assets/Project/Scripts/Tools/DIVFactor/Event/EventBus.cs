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
    /// <summary>
    /// イベントから別のイベントへ処理を連結させるためのイベントオブジェクト
    /// </summary>
    public class EventChannel<TReq, TRes>
    where TReq : EventObject
    where TRes : EventObject
    {
        EventBus<TReq> Request { get; init; }
        EventBus<TRes> Response { get; init; }

        public EventChannel(EventBus<TReq> req, EventBus<TRes> res)
        {
            Request = req;
            Response = res;
        }
        /// <summary>
        /// イベントを受信した際のコールバックを設定する
        /// </summary>
        /// <param name="func">コールバック</param>
        /// <returns>受信側のDisposable</returns>
        public IDisposable Subscribe(Func<TReq, TRes> func)
        {
            return Request.Subscribe(req => Response.OnNext(func(req)));
        }
    }
}