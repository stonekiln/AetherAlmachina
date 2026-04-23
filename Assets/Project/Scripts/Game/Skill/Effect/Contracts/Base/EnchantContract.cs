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
        /// <param name="removeModifier">Modifierの解除動作を行う関数</param>
        public abstract void Sign(IEntityInteraction user, IEntityInteraction target, Action removeModifier);
    }
    /// <summary>
    /// Modifierの解除を行うための情報を渡す
    /// </summary>
    /// <param name="User">使用者</param>
    /// <param name="Target">付与対象</param>
    /// <param name="Remove">Modifierの解除動作を行う関数</param>
    public record DispelModifier(IEntityInteraction User, IEntityInteraction Target, Action Remove)
    {
        /// <summary>
        /// Modifierの解除の予約を実行する
        /// </summary>
        /// <param name="contract">解除のタイミング</param>
        public void Execute(EnchantContract contract)
        {
            contract.Sign(User, Target, Remove);
        }
    }

    public static class DispelModifierExtensions
    {
        public static void Signed(this DispelModifier dispel, EnchantContract contract)
        {
            dispel.Execute(contract);
        }
    }
}