using System;
using AetherAlmachina.Entities;
using R3;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    /// <summary>
    /// 付与対象者が指定した回数の行動後にModifierが解除される
    /// </summary>
    [Serializable]
    public class SkillTimesContract : EnchantContract
    {
        public override void Sign(IEntityInteraction user, IEntityInteraction target, Action removeModifier)
        {
            target.Process.SkillEnd.Skip((int)During).Take(1).Subscribe(_ => removeModifier());
        }
    }
}