using System;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// スキルの効果の実行部分を担うクラス
    /// </summary>
    public abstract class SkillEffect
    {
        public abstract Type ParameterType { get; }
        /// <summary>
        /// スキルの効果を実行する(1対1対応)
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">対象者</param>
        /// <param name="parameter">エフェクトの設定値</param>
        public abstract void Apply(IEntityInteraction user, IEntityInteraction target, EffectParameter parameter);
    }
    /// <summary>
    /// ジェネリックでパラメータと実行部を紐づける
    /// </summary>
    /// <typeparam name="TParameter">使用するパラメータ</typeparam>
    public abstract class SkillEffect<TParameter> : SkillEffect where TParameter : EffectParameter
    {
        public sealed override Type ParameterType => typeof(TParameter);
        public sealed override void Apply(IEntityInteraction user, IEntityInteraction target, EffectParameter parameter)
        {
            ApplyTyped(user, target, (TParameter)parameter);
        }
        /// <summary>
        /// スキルの効果を実行する(1対1対応)(引数とするパラメータを指定した型にキャスト済み)
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">対象者</param>
        /// <param name="parameter">エフェクトの設定値</param>
        protected abstract void ApplyTyped(IEntityInteraction user, IEntityInteraction target, TParameter parameter);
    }
    /// <summary>
    /// エフェクトの実行に必要なパラメータ
    /// </summary>
    public abstract class EffectParameter
    {

    }
}