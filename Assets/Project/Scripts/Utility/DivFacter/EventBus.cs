using System;
using R3;

namespace DivFacter.Event
{
    /// <summary>
    /// イベントを発行するためのクラス
    /// </summary>
    /// <typeparam name="T">イベントメッセージ</typeparam>
    public class EventBus<T> where T : EventObject
    {
        /// <summary>
        /// イベント本体
        /// </summary>
        readonly Subject<T> Event = new();
        /// <summary>
        /// Tのイベントを監視する
        /// </summary>
        /// <param name="action">イベントが発行されたときに実行する関数</param>
        /// <returns></returns>
        public IDisposable Subscribe(Action<T> action) => Event.Subscribe(action);
        /// <summary>
        /// Tのイベントを発行する
        /// </summary>
        /// <param name="value">イベントメッセージ</param>
        public void Publish(T value) => Event.OnNext(value);
    }

    /// <summary>
    /// 全てのイベントメッセージはこれを継承すること
    /// </summary>
    public abstract record EventObject;
}