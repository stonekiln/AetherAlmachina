using System;
using AetherAlmachina.Entities;
using R3;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    [Serializable]
    public class SkillTimesContract : EnchantContract
    {
        public override void Sign(ICombatInteraction user, ICombatInteraction target, Action removeModifier)
        {
            target.Command.SkillEnd.Skip((int)During).Take(1).Subscribe(_ => removeModifier());
        }
    }
}