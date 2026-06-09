using System;
using System.Collections.Generic;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect
{
    /// <summary>
    /// スキルの効果の実行用データ
    /// </summary>
    /// <param name="User">使用者</param>
    /// <param name="Targets">効果対象</param>
    /// <param name="SkillData">そのエフェクトの親となるスキル効果</param>
    public record SkillExecutionContext(IEntityInteraction User, IEnumerable<IEntityInteraction> Targets, ActivatedSkillData SkillData);
    /// <summary>
    /// スキルの効果の実行部分を担うクラス
    /// </summary>
    public abstract class SkillEffect
    {
        public abstract Type ParameterType { get; }
        /// <summary>
        /// スキルの効果を実行する(1対1対応)
        /// </summary>
        /// <param name="context">実行用コンテキスト</param>
        public abstract void Apply(SkillExecutionContext context, EffectParameter parameter);
    }
    /// <summary>
    /// ジェネリックでパラメータと実行部を紐づける
    /// </summary>
    /// <typeparam name="TParam">使用するパラメータ</typeparam>
    public abstract class SkillEffect<TParam> : SkillEffect where TParam : EffectParameter
    {
        public sealed override Type ParameterType => typeof(TParam);
        public sealed override void Apply(SkillExecutionContext context, EffectParameter parameter)
        {
            ApplyTyped(context, (TParam)parameter);
        }
        /// <summary>
        /// スキルの効果を実行する(1対1対応)(引数とするパラメータを指定した型にキャスト済み)
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">対象者</param>
        /// <param name="parameter">エフェクトの設定値</param>
        protected abstract void ApplyTyped(SkillExecutionContext context, TParam parameter);
    }

    /// <summary>
    /// エフェクトの実行に必要なパラメータ
    /// </summary>
    public abstract class EffectParameter { }
}
