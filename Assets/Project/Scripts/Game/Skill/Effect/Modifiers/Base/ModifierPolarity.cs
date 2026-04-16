using System;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ModifierPolarity
    {
        public abstract string DisplaySign { get; }
        public abstract float ParameterMax { get; }
        public abstract float ParameterMin { get; }
    }

    [Serializable]
    public class PositiveModifier : ModifierPolarity
    {
        public override string DisplaySign => "+";
        public override float ParameterMax => float.PositiveInfinity;
        public override float ParameterMin => 0;
    }

    [Serializable]
    public class NegativeModifier : ModifierPolarity
    {
        public override string DisplaySign => "-";
        public override float ParameterMax => 0;
        public override float ParameterMin => float.NegativeInfinity;
    }
}