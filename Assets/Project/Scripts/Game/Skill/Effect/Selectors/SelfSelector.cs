using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Selectors
{
    /// <summary>
    /// 使用者自身を選択するセレクター
    /// </summary>
    [Serializable]
    public class SelfSelector : Selector
    {
        public override bool IsDeferrable => false;
        public override IEnumerable<IEntityInteraction> Targeting(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, int index)
        {
            List<IEntityInteraction> list = friendly.ToList();
            (list[0], list[index]) = (list[index], list[0]);
            return list;
        }
    }
}