namespace DIVFactor.Event
{
    /// <summary>
    /// 全てのイベントメッセージはこれを継承すること
    /// </summary>
    public abstract record EventObject;
    /// <summary>
    /// MonoBehaviourのライフサイクルが始まる直前に発行されるイベント
    /// </summary>
    public record ActivateEvent : EventObject;
}