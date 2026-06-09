using System;
using R3;

namespace AetherAlmachina.Skill.Effect.Contracts
{
    /// <summary>
    /// 付与対象者が指定した回数の行動後にModifierが解除される
    /// </summary>
    [Serializable]
    public class SkillTimesContract : EnchantContract
    {
        public override Observable<Unit> Create(EnchantExecutionContext context)
        {
            return context.Target.Interaction.SkillEnd.Skip((int)During).Take(1).AsUnitObservable();
        }
    }
}