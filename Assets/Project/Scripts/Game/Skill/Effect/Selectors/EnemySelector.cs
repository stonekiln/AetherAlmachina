using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;
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
        public override IEnumerable<IEntityInteraction> Targeting(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, Vector2Int layoutIndex)
        {
            return hostile;
        }
    }
}