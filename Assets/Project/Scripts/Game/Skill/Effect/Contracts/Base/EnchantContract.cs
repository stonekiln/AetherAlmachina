using System;
using UnityEngine;
using AetherAlmachina.Entities;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    public abstract class EnchantContract
    {
        [field: SerializeField] protected float During { get; private set; }

        public abstract void Sign(ICombatInteraction user, ICombatInteraction target, Action removeModifier);
    }

    public record DispelModifier(ICombatInteraction User, ICombatInteraction Target, Action Remove)
    {
        public void Execute(EnchantContract contract)
        {
            contract.Sign(User, Target, Remove);
        }
    }

    public static class DispelModifierExtensions
    {
        public static void Signed(this DispelModifier dispel, EnchantContract contract)
        {
            dispel.Execute(contract);
        }
    }
}