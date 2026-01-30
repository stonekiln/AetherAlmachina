using System;
using R3;
using VContainer;

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
    /// イベントを発行しそれに対する返信を入手するためのクラス
    /// </summary>
    /// <typeparam name="TReq">イベント発行用の型</typeparam>
    /// <typeparam name="TRes">イベント返信用の型</typeparam>
    public abstract class EventChannel<TReq, TRes>
    where TReq : EventObject
    where TRes : EventObject
    {
        EventBus<TReq> RequestEvent { get; set; }
        EventBus<TRes> ResponseEvent { get; set; }

        [Inject]
        void Construct(EventBus<TReq> request, EventBus<TRes> response)
        {
            RequestEvent = request;
            ResponseEvent = response;
        }
        /// <summary>
        /// イベントを発行する
        /// </summary>
        /// <param name="req">イベントメッセージ</param>
        public void Call(TReq req) => RequestEvent.OnNext(req);
        /// <summary>
        /// 発行したイベントの返信を監視する
        /// </summary>
        /// <param name="response">リプライメッセージ</param>
        /// <returns></returns>
        public IDisposable Response(Action<TRes> response) => ResponseEvent.Subscribe(response);
        /// <summary>
        /// イベント受信時の処理を設定する
        /// </summary>
        /// <param name="reply">処理の内容</param>
        /// <returns></returns>
        public IDisposable Reply(Func<TReq, TRes> reply) => RequestEvent.Subscribe(request => ResponseEvent.OnNext(reply(request)));
    }
}