using System;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class FlatModifier : CommonModifier
    {
        public override string DisplayUnit => "";
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
    }

    public abstract class PositiveFlatModifier : FlatModifier
    {
        public override float ParameterMax => float.PositiveInfinity;
        public override float ParameterMin => 0;
        public override string DisplaySign => "+";
    }

    public abstract class NegativeFlatModifier : FlatModifier
    {
        public override float ParameterMax => 0;
        public override float ParameterMin => float.NegativeInfinity;
        public override string DisplaySign => "-";
    }
}