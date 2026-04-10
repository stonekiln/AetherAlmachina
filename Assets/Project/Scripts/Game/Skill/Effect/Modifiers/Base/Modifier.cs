using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public interface INonSliderRange
    {
        public float ParameterMax { get; }
        public float ParameterMin { get; }
        public string DisplayUnit { get; }
    }

    public abstract class ModifierBase : INonSliderRange
    {
        public abstract float ParameterMax { get; }
        public abstract float ParameterMin { get; }
        public abstract string DisplayUnit { get; }
        protected abstract Type ModifierParameterKey { get; }
        public abstract StatusType StatusTypeKey { get; }

        public abstract DispelModifier Enchant(ICombatInteraction user, ICombatInteraction target, IModifierData modifierData);
    }

    public abstract class CommonModifier : ModifierBase
    {
        public override DispelModifier Enchant(ICombatInteraction user, ICombatInteraction target, IModifierData modifierData)
        {
            Action remove = target.Status.Modifiers[ModifierParameterKey].AddModifier(modifierData);
            return new(user, target, remove);
        }
    }

    public abstract class PositiveModifier : CommonModifier
    {
        public override float ParameterMax => float.PositiveInfinity;
        public override float ParameterMin => 0;
    }

    public abstract class NegativeModifier : CommonModifier
    {
        public override float ParameterMax => 0;
        public override float ParameterMin => float.NegativeInfinity;
    }

    public abstract class PositiveRateModifier : PositiveModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "%";
    }

    public abstract class NegativeRateModifier : NegativeModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "%";
    }

    public abstract class PositiveFlatModifier : PositiveModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }

    public abstract class NegativeFlatModifier : NegativeModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }
}