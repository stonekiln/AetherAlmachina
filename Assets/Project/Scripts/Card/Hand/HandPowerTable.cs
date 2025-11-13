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
            1 => stack.table[count - 1],
            2 => chain.table[count - 1],
            _ => 1,
        };
    }
}

[Serializable]
public class StackTable
{
    [NonSerialized] public float[] table;
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
public class ChainTable
{
    [NonSerialized] public float[] table;
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