using System.IO;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public int cost;
    public float power;
    public Sprite icon;
    public Entity owner;
    public Entity target;

    public GameObject CreateObject()
    {
        GameObject cardObject = Instantiate(Resources.Load<GameObject>(Path.Combine("SkillCard", "CardBase")));
        cardObject.transform.GetChild(0).GetComponent<CardData>().Initialize(icon, cost);
        cardObject.transform.GetChild(1).GetComponent<CardSelecter>().onClickCallback = () => Activate();
        return cardObject;
    }
    public abstract void Activate();
}