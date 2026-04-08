using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class RateModifier : CommonModifier
    {
        public override string DisplayUnit => "%";
        protected override Type ModifierParameterKey => typeof(RateModifierParameter);
    }

    public abstract class PositiveRateModifier : RateModifier
    {
        public override float ParameterMax => float.PositiveInfinity;
        public override float ParameterMin => 1;
    }
    public abstract class NegativeRateModifier : RateModifier
    {
        public override float ParameterMax => 1;
        public override float ParameterMin => float.NegativeInfinity;
    }
}