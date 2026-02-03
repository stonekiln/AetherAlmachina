using SKill;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [SerializeField] protected string skillName;
    public int cost;
    [SerializeField] protected float power;
    public Sprite icon;
    public int maxTargeting = 1;
    public abstract void Activate(Entity owner, Entity target);
    public abstract TargetFlag TargetData();
}