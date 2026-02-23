using System;
using UnityEngine;

public interface IAttackSkill
{
    AttackSkillParam AttackSkill { get; }
}

[Serializable]
public class AttackSkillParam
{
    public int MaxTargeting = 1;
    public float Power = 1;
    public Entity Owner { get; set; }

    public void Activate(Entity target)
    {
        Debug.Log(true);
        Owner.Attack(target, Power);
    }
}