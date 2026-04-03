using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public abstract class Modifier
    {
        public abstract void Enchant(ICombatInteraction target, float value, float during);
    }
    public abstract class FlatModifier : Modifier
    {
        public override void Enchant(ICombatInteraction target, float value, float during)
        {
            EnchantTyped(target.Status.FlatModifier, GetType(), value, during);
        }
        public abstract void EnchantTyped(FlatModifierParameter buff, Type modifierType, float value, float during);
    }
    public abstract class PercentModifier : Modifier
    {
        public override void Enchant(ICombatInteraction target, float value, float during)
        {
            EnchantTyped(target.Status.PercentModifier, GetType(), value, during);
        }
        public abstract void EnchantTyped(PercentModifierParameter buff, Type modifierType, float value, float during);
    }
}