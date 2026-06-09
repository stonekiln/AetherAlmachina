using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Skill.Effect.Contracts;
using AetherAlmachina.Skill.Effect.Modifiers;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    public record EnchantExecutionContext(IEntityInteraction User, IEntityInteraction Target, ActivatedSkillData SkillData);

    /// <summary>
    /// 相手にModifierを付与する効果
    /// </summary>
    [Serializable]
    public class EnchantEffect : SkillEffect<EnchantParameter>
    {
        protected override void ApplyTyped(SkillExecutionContext context, EnchantParameter parameter)
        {
            foreach (IEntityInteraction target in context.Targets)
            {
                EnchantExecutionContext enchantContext = new(context.User, target, context.SkillData);
                Action dispel = parameter.EnchantData.Definition.ApplyModifier(enchantContext, parameter.EnchantData.RawData);
                Observable<Unit> contractFromDefinition = parameter.EnchantData.Definition.Create(enchantContext);
                Observable<Unit> contractFromParameter = parameter.Contract.Create(enchantContext);

                Observable.Merge(contractFromParameter, contractFromDefinition).Take(1).Subscribe(_ => dispel()).AddTo((Entity)target);
            }
        }
    }

    /// <summary>
    /// Modifierの種類と効果量をインスペクター上で指定できるようにする
    /// </summary>
    [Serializable]
    public class ModifierEnchantData
    {
        [field: SerializeField] ModifierAsset DefinitionAsset { get; set; }
        [field: SerializeField] float Value { get; set; }
        public ModifierBase Definition => DefinitionAsset.Definition;
        public ModifierRawData RawData => new(DefinitionAsset, Value);
    }
    /// <summary>
    /// EnchantEffectに必要なパラメータ
    /// </summary>
    [Serializable]
    public class EnchantParameter : EffectParameter
    {
        /// <summary>
        /// Modifierの情報
        /// </summary>
        [field: SerializeField] public ModifierEnchantData EnchantData { get; private set; }
        /// <summary>
        /// エフェクト解除のタイミング
        /// </summary>
        [field: SerializeReference] public EnchantContract Contract { get; private set; }
    }
}
