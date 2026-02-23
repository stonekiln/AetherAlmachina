using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public int Cost { get; init; }
    public Sprite Icon { get; init; }
    readonly Action<IEnumerable<ICombatInteraction>, IEnumerable<ICombatInteraction>> targeting;

    public SkillData(SkillBase skillBase, Entity owner)
    {
        Cost = skillBase.cost;
        Icon = skillBase.icon;
        targeting = (friendly, hostile) => skillBase.Targeting(friendly, hostile);
        if (skillBase is IAttackSkill attackSkill) attackSkill.AttackSkill.Owner = owner;
    }
    public void Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile) => targeting(friendly, hostile);
}