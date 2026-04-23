using System.Collections.Generic;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Selectors
{
    /// <summary>
    /// スキルエフェクトの効果対象を定義するためのクラス
    /// </summary>
    public abstract class Selector
    {
        /// <summary>
        /// 効果対象の選別を行う
        /// </summary>
        /// <param name="friendly">友好勢力</param>
        /// <param name="hostile">敵対勢力</param>
        /// <param name="index">使用者のインデックス</param>
        /// <returns>効果対象</returns>
        public abstract IEnumerable<IEntityInteraction> Targeting(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, int index);
    }
}