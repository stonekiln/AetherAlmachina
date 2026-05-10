using System.Collections.Generic;
using AetherAlmachina.Entities.Faction;
using DIVFactor.Event;

namespace DConfig.StageLife.Event
{
    /// <summary>
    /// 味方側エンティティの整列をリクエストするイベント
    /// </summary>
    /// <param name="Friendly">味方側エンティティのリスト</param>
    public record FriendlyLayoutEvent(List<Player> Friendly) : EventObject;
    /// <summary>
    /// 敵側エンティティの整列をリクエストするイベント
    /// </summary>
    /// <param name="Hostile">敵側エンティティのリスト</param>
    public record HostileLayoutEvent(List<Enemy> Hostile) : EventObject;
}
