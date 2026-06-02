using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
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
        /// <param name="userIndex">使用者のインデックス</param>
        /// <returns>効果対象</returns>
        public IEnumerable<IEntityInteraction> Targeting(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, Vector2Int userIndex)
        {
            return SelectTarget(friendly.OrderBy(entity => entity.Status.Get(StatusType.Speed)), hostile.OrderBy(entity => entity.Status.Get(StatusType.Speed)), userIndex);
        }
        /// <summary>
        /// 各種Selectorで固有のフィルタリングを行う
        /// </summary>
        /// <param name="friendly">速度の小さい順に並び変えた友好勢力</param>
        /// <param name="hostile">速度の小さい順に並び変えた敵対勢力</param>
        /// <param name="userIndex">使用者のインデックス</param>
        /// <returns>効果対象</returns>
        //速度の低いエンティティは狙われやすくなる
        public abstract IEnumerable<IEntityInteraction> SelectTarget(IEnumerable<IEntityInteraction> friendly, IEnumerable<IEntityInteraction> hostile, Vector2Int userIndex);
    }
}