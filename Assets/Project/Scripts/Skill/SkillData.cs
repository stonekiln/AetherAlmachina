using System;
using UnityEngine;

public class SkillData
{
    public Action Activate { get; private set; }
    public Action<GameObject> SetOwner { get; private set; }
    public Func<GameObject> Create { get; private set; }

    public SkillData(SkillBase skill)
    {
        Activate = () => skill.Activate();
        SetOwner = (ownerObject) => skill.SetOwner(ownerObject);
        Create = () => skill.CreateObject();
    }
}