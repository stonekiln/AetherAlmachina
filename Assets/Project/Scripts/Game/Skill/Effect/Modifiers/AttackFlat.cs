using System;
using AetherAlmachina.Entities.Parameter;
using R3;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [Serializable]
    public class AttackFlat : FlatModifier
    {
        public override void EnchantTyped(FlatModifierParameter buff, Type modifierType, float value, float during)
        {
            buff.AddModifier(modifierType, StatusType.Attack, value);
            Observable.Timer(TimeSpan.FromSeconds(during)).Subscribe(_ => buff.RemoveModifier(modifierType, StatusType.Attack, value));
        }
    }
}