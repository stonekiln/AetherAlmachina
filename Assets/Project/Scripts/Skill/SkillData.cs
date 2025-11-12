using System;
using UnityEngine;

public class SkillData
{
    public Action Activate { get; private set; }
    public Func<GameObject> Create { get; private set; }

    public SkillData(Action action, Func<GameObject> create)
    {
        Activate = action;
        Create = create;
    }
}