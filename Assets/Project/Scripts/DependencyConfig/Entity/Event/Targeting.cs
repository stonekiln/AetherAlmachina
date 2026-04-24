using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public record TargetingEventBundle(LockOnEvent LockOn, EventBus<HitEvent> Hit);

    /// <summary>
    /// ロックオンを行うためのイベントオブジェクト
    /// </summary>
    public class LockOnEvent : EventChannel<LockOnRequestEvent, LockOnResponseEvent> { };
    /// <summary>
    /// ロックオンを宣言するイベントメッセージ
    /// </summary>
    /// <param name="Selector"></param>
    public record LockOnRequestEvent(Func<IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>, IEnumerable<IEntityInteraction>> Selector) : EventObject;
    /// <summary>
    /// ロックオンの結果を渡すためのイベントメッセージ
    /// </summary>
    /// <param name="Targets"></param>
    public record LockOnResponseEvent(IEnumerable<IEntityInteraction> Targets) : EventObject;
    /// <summary>
    /// スキルエフェクトの着弾を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Apply"></param>
    public record HitEvent(Action<Entity> Apply) : EventObject;
}