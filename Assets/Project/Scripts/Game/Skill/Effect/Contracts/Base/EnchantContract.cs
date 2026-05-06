using System;
using UnityEngine;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    /// <summary>
    /// Modifierの解除のタイミングを事前に決めるためのクラス
    /// </summary>
    public abstract class EnchantContract
    {
        [field: SerializeField] protected float During { get; private set; }
        /// <summary>
        /// Modifierの解除の予約を実行する
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象者</param>
        /// <param name="dispel">Modifierの解除動作を行う関数</param>
        public abstract void Sign(IEntityInteraction user, IEntityInteraction target, Action dispel);
    }
    /// <summary>
    /// Modifierの解除を行うための情報を渡す
    /// </summary>
    public class DispelModifier
    {
        IEntityInteraction User { get; init; }
        IEntityInteraction Target { get; init; }
        Action Dispel { get; init; }

        public DispelModifier(IEntityInteraction user, IEntityInteraction target, Action dispel)
        {
            User = user;
            Target = target;
            Dispel = dispel;
        }

        /// <summary>
        /// Modifierの解除の予約を実行する
        /// </summary>
        /// <param name="contract">解除のタイミング</param>
        public void Signed(EnchantContract contract)
        {
            contract.Sign(User, Target, Dispel);
        }
    }
}