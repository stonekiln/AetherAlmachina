using System;
using System.IO;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [SerializeField] private string skillName;
    [SerializeField] private int cost;
    [SerializeField] private float power;
    [SerializeField] private Sprite icon;
    [NonSerialized] public Entity owner;

    public string SkillName => skillName;
    public int Cost => cost;
    public float Power => power;
    public Sprite Icon => icon;

    public GameObject CreateObject()
    {
        GameObject cardObject = Instantiate(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")));
        cardObject.GetComponent<CardManager>().Initialize(Cost, Icon, () => Activate());
        return cardObject;
    }
    public void SetOwner(GameObject ownerObject)
    {
        owner = ownerObject.GetComponent<Entity>();
    }
    public abstract void Activate();
}