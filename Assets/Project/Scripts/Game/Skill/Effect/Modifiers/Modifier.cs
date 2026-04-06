using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class ModifierBase
    {
        protected abstract Type ModifierParameterKey { get; }
        protected abstract StatusType StatusTypeKey { get; }
        public abstract DispelModifier Enchant(ICombatInteraction user, ICombatInteraction target, ModifierAsset modifierAsset, float value);
    }

    public abstract class CommonModifier : ModifierBase
    {
        public override DispelModifier Enchant(ICombatInteraction user, ICombatInteraction target, ModifierAsset modifierAsset, float value)
        {
            Action remove = target.Status.Modifiers[ModifierParameterKey].AddModifier(modifierAsset, StatusTypeKey, value);
            return new(user, target, remove);
        }
    }

    public abstract class FlatModifier : CommonModifier
    {
        protected override Type ModifierParameterKey => typeof(FlatModifierParameter);
    }

    public abstract class PercentModifier : CommonModifier
    {
        protected override Type ModifierParameterKey => typeof(PercentModifierParameter);
    }
}