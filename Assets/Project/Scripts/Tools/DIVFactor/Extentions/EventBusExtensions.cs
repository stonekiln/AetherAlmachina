using DIVFactor.Event;

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
        /// <returns></returns>
        public static EventChannel<TReq, TRes> Switch<TReq, TRes>(this EventBus<TReq> current, EventBus<TRes> next)
            where TReq : EventObject
            where TRes : EventObject
        {
            return new(current, next);
        }
    }
}