using System;
using DIVFactor.Event;

namespace DIVFactor.Lifecycle
{
    public record ReleaseRequest : RequestEvent;
    public record DestroyRequest : RequestEvent;
    /// <summary>既存の型付きファクトリーと引数を閉じ込めた生成要求。</summary>
    public record SpawnRequest(Action Factory) : RequestEvent;

    [EventHub]
    public record LifecycleEventHub(
        EventPort<ReleaseRequest> Release,
        EventPort<DestroyRequest> Destroy,
        EventPort<SpawnRequest> Spawn);
}
