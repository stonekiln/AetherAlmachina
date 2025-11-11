using System;
using UnityEngine;

public class SkillData
{
    public Action Activate { get; private set; }
    public GameObject CardObject { get; private set; }

    public SkillData(Action action, GameObject cardObject)
    {
        Activate = action;
        CardObject = cardObject;
    }
}