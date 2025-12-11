using System.IO;
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

    public GameObject CreateObject()
    {
        GameObject cardObject = Instantiate(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")));
        return cardObject;
    }
    public void SetOwner(Entity ownerEntity)
    {
        owner = ownerEntity;
    }
    public abstract void Activate();
}