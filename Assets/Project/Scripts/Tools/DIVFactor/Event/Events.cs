using DIVFactor.Lifetime;

namespace DIVFactor.Event
{
    /// <summary>
    /// 全てのイベントメッセージはこれを継承すること
    /// </summary>
    public abstract record EventObject;
    /// <summary>
    /// MonoBehaviourのライフサイクルが始まる直前に発行されるイベント
    /// </summary>
    public record ActivateEvent() : EventObject;
    /// <summary>
    /// バインドを実行する際に発行されるイベント
    /// </summary>
    /// <param name="Lifetime">バインドを実行したLifetimeObject</param>
    public record BindEvent(LifetimeObject Lifetime):EventObject;
    /// <summary>
    /// LifetimeScopeがDestroyされるときに発行されるイベント
    /// </summary>
    public record DestroyEvent:EventObject;
}