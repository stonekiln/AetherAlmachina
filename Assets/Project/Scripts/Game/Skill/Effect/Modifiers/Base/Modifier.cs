using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ModifierBase
    {
        public abstract float ParameterMax { get; }
        public abstract float ParameterMin { get; }
        protected abstract Type ModifierParameterKey { get; }
        public abstract StatusType StatusTypeKey { get; }
        public abstract string DisplayUnit { get; }
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