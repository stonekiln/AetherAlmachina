using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public interface IModifierUnit
    {
        public string DisplayUnit { get; }
    }

    public abstract class ModifierBase : IModifierUnit
    {
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

    public abstract class FlatModifier : CommonModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
        public override string DisplayUnit => "";
    }

    public abstract class RateModifier : CommonModifier
    {
        protected override Type ModifierParameterKey => typeof(RateModifierParameter);
        public override string DisplayUnit => "%";
    }
}