using System.Collections.Generic;
using AetherAlmachina.Entities;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Selectors
{
    /// <summary>
    /// スキルエフェクトの効果対象を定義するためのクラス
    /// </summary>
    public abstract class Selector
    {
        /// <summary>
        /// 遅延可能なセレクターかどうか
        /// </summary>
        //基本的に相手を対象にとる場合trueを設定する
        public abstract bool IsDeferrable { get; }
        /// <summary>
        /// 効果対象の選別を行う
        /// </summary>
        /// <param name="friendly">友好勢力</param>
        /// <param name="hostile">敵対勢力</param>
        /// <param name="layoutIndex">使用者のインデックス</param>
        /// <returns>効果対象</returns>
        public abstract IEnumerable<IEntityInteraction> Targeting(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, Vector2Int layoutIndex);
    }
}