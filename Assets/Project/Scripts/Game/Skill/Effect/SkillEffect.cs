using System;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect
{
    public abstract class SkillEffect
    {
        public abstract Type ParameterType { get; }
        public abstract void Apply(IEntityInteraction user, IEntityInteraction target, EffectParameter parameter);
    }
    public abstract class SkillEffect<TParameter> : SkillEffect where TParameter : EffectParameter
    {
        public sealed override Type ParameterType => typeof(TParameter);
        public sealed override void Apply(IEntityInteraction user, IEntityInteraction target, EffectParameter parameter)
        {
            ApplyTyped(user, target, (TParameter)parameter);
        }
        protected abstract void ApplyTyped(IEntityInteraction user, IEntityInteraction target, TParameter parameter);
    }
    public abstract class EffectParameter
    {

    }
}