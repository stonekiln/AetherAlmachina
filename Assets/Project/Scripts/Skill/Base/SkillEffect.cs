using System;
using UnityEngine;

namespace Skill
{
    public abstract class SkillEffect : ScriptableObject
    {
        public abstract Type ParameterType { get; }
        public abstract void Apply(ICombatInteraction user, ICombatInteraction target, EffectParameter parameter);
    }
    public abstract class SkillEffect<TParameter> : SkillEffect where TParameter : EffectParameter
    {
        public sealed override Type ParameterType => typeof(TParameter);
        public sealed override void Apply(ICombatInteraction user, ICombatInteraction target, EffectParameter parameter)
        {
            ApplyTyped(user, target, (TParameter)parameter);
        }
        protected abstract void ApplyTyped(ICombatInteraction user, ICombatInteraction target, TParameter parameter);
    }
    public abstract class EffectParameter
    {

    }
}