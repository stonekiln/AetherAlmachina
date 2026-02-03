using System;
using SKill;
using UnityEngine;

namespace SKill
{
    [Flags]
    public enum TargetFlag
    {
        Friendly = 1,
        Hostile = 2,
    }
}

public class SkillData
{
    public int Cost { get; init; }
    public Sprite Icon { get; init; }
    public TargetFlag Target { get; init; }
    public int MaxTargeting { get; init; }
    readonly Action<Entity> action;

    public SkillData(SkillBase skillBase, Entity owner)
    {
        Cost = skillBase.cost;
        Icon = skillBase.icon;
        Target = skillBase.TargetData();
        MaxTargeting = skillBase.maxTargeting;
        action = (target) => skillBase.Activate(owner, target);
    }
    public void Activate(Entity target) => action(target);
}