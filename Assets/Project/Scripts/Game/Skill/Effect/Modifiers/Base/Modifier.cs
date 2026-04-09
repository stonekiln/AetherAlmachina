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
        public string DisplaySign { get; }
    }

    public abstract class ModifierBase : INonSliderRange
    {
        public abstract float ParameterMax { get; }
        public abstract float ParameterMin { get; }
        public abstract string DisplayUnit { get; }
        public abstract string DisplaySign { get; }
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
}