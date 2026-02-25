using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Activate(Entity user, Entity target);
}