using DIVFactor.Lifetime;

namespace DIVFactor.Event
{
    /// <summary>
    /// バインドを実行する際に発行されるイベント
    /// </summary>
    /// <param name="Lifetime">バインドを実行したLifetimeObject</param>
    public record BindEvent(LifetimeObject Lifetime) : EventObject;
    /// <summary>
    /// MonoBehaviourのライフサイクルが始まる直前に発行されるイベント
    /// </summary>
    public record ActiveEvent() : EventObject;
    /// <summary>
    /// LifetimeScopeがDestroyされるときに発行されるイベント
    /// </summary>
    public record EndEvent : EventObject;
    /// <summary>
    /// 各種EventPointを実装する
    /// </summary>
    public record EventPoint(EventBus<BindEvent> BindPoint,EventBus<ActiveEvent> ActivePoint,EventBus<EndEvent> EndPoint);
}