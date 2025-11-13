using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HandPowerTable", menuName = "GameSettings/HandPowerTable")]
public class HandPowerTable : ScriptableObject
{
    [SerializeField] private StackTable stack;
    [SerializeField] private ChainTable chain;
    public StackTable Stack => stack;
    public ChainTable Chain => chain;
}

public class TableBase
{
    protected float[] table;
    public float Level(int value)
    {
        return table[value - 1];
    }
}
[Serializable]

public class StackTable : TableBase
{
    [SerializeField] float level1;
    [SerializeField] float level2;
    [SerializeField] float level3;
    [SerializeField] float level4;
    public StackTable()
    {
        table = new float[] { level1, level2, level3, level4 };
    }
}

[Serializable]
public class ChainTable : TableBase
{
    [SerializeField] float level1;
    [SerializeField] float level2;
    [SerializeField] float level3;
    [SerializeField] float level4;
    [SerializeField] float level5;
    public ChainTable()
    {
        table = new float[] { level1, level2, level3, level4, level5 };
    }
}