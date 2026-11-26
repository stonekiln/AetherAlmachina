using AetherAlmachina.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Selectors
{
    /// <summary>
    /// 敵を選択するセレクター
    /// </summary>
    [Serializable]
    public class EnemySelector : Selector
    {
        public override bool IsDeferrable => true;

        public override IEnumerable<IEntityInteraction> SelectTarget(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, Vector2Int userIndex)
        {
            return hostile;
        }
    }
}