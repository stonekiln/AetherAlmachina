using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Selectors
{
    /// <summary>
    /// 使用者自身を選択するセレクター
    /// </summary>
    [Serializable]
    public class SelfSelector : Selector
    {
        public override bool IsDeferrable => false;

        public override IEnumerable<IEntityInteraction> Targeting(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, Vector2Int layoutIndex)
        {
            List<IEntityInteraction> list = friendly.ToList();
            for (int i = 0; i < friendly.Count(); i++)
            {
                if (list[i].LayoutIndex == layoutIndex)
                {
                    IEntityInteraction entity = list[i];

                    list.RemoveAt(i);
                    list.Insert(0, entity);

                    return list;
                }
            }

            return list;
        }
    }
}