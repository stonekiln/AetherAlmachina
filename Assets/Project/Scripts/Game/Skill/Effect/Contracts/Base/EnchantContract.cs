using UnityEngine;
using R3;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    public interface IContractFactory
    {
        /// <summary>
        /// それぞれのModifierで固有の解除の条件を定義する
        /// </summary>
        /// <param name="context">Contractを作成するための周辺情報</param>
        /// <returns>解除条件</returns>
        Observable<Unit> Create(EnchantExecutionContext context);
    }
    /// <summary>
    /// Modifierの解除のタイミングを事前に決めるためのクラス
    /// </summary>
    public abstract class EnchantContract : IContractFactory
    {
        [field: SerializeField] protected float During { get; private set; }

        public abstract Observable<Unit> Create(EnchantExecutionContext context);
    }
}