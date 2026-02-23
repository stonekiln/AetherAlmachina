using System;

public interface IBuffSkill
{
    BuffSkillParam BuffSkill { get; }
}

[Serializable]
public class BuffSkillParam
{
    public int MaxTargeting = 1;
    public float Power = 1;
    public Entity Owner { get; set; }

    public void Activate(Entity target)
    {
        Owner.Attack(target, Power);
    }
}