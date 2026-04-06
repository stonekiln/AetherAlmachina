using System;
using AetherAlmachina.Entities;
using R3;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    [Serializable]
    public class TimeContract : EnchantContract
    {
        public override void Sign(ICombatInteraction user, ICombatInteraction target, Action removeModifier)
        {
            Observable.Timer(TimeSpan.FromSeconds(During)).Subscribe(_ => removeModifier());
        }
    }
}