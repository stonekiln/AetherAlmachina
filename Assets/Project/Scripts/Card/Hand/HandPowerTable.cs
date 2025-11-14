using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HandPowerTable", menuName = "GameSettings/HandPowerTable")]
public class HandPowerTable : ScriptableObject
{
    [SerializeField] StackTable stack;
    [SerializeField] ChainTable chain;
    public float Get(int type, int count)
    {
        return type switch
        {
            1 => stack.Get(count),
            2 => chain.Get(count),
            _ => 1,
        };
    }
}

[Serializable]
public class StackTable
{
    const float Default = 1f;
    [SerializeField] float level2;
    [SerializeField] float level3;
    [SerializeField] float level4;
    public virtual float Get(int level)
    {
        return level switch
        {
            1 => Default,
            2 => level2,
            3 => level3,
            4 => level4,
            _ => Default,
        };
    }
}

[Serializable]
public class ChainTable : StackTable
{
    [SerializeField] float level5;
    public override float Get(int level)
    {
        return level switch
        {
            5 => level5,
            _ => base.Get(level),
        };
    }
}