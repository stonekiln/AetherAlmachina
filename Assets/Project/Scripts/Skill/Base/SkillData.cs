using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [SerializeField] protected string skillName;
    [SerializeField] int cost;
    [SerializeField] protected float power;
    [SerializeField] Sprite icon;
    protected Entity owner;
    public int Cost => cost;
    public Sprite Icon => icon;

    public SkillData SetOwner(Entity ownerEntity)
    {
        owner = ownerEntity;
        return this;
    }
    public abstract void Activate();
}