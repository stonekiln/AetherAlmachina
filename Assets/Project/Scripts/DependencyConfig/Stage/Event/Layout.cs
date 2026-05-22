using System.Collections.Generic;
using AetherAlmachina.Entities.Faction;
using DIVFactor.Event;
using UnityEngine;

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
    /// <summary>
    /// レイアウトのインデックスを通知するイベント
    /// </summary>
    /// <param name="Index">レイアウトされているときのインデックス</param>
    // HACK: 現時点でレイアウトインデックスが頻繁に変わるケースがないため、送信専用で、SiblingIndexからの変換は考えない
    public record LayoutIndexEvent(Vector2Int Index) : EventObject;
}
