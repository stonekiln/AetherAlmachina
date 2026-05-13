using DIVFactor.Event;
using R3;

namespace DIVFactor.Extensions
{
    static class EventBusExtensions
    {
        /// <summary>
        /// イベントを監視し別のイベントを発火するためのイベントオブジェクトを作成する
        /// </summary>
        /// <typeparam name="TReq">受信用イベントメッセージ</typeparam>
        /// <typeparam name="TRes">発振用イベントメッセージ</typeparam>
        /// <param name="current">受信用イベント</param>
        /// <param name="next">発振用イベント</param>
        /// <returns>作成されたイベントオブジェクト</returns>
        public static EventChannel<TReq, TRes> Switch<TReq, TRes>(this Observable<TReq> current, Subject<TRes> next)
        {
            return new(current, next);
        }
    }
}