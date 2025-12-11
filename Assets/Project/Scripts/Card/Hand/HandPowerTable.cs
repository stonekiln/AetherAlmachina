using System;
using UnityEngine;

/// <summary>
/// 役の強さを設定するパラメータ
/// </summary>
[CreateAssetMenu(fileName = "HandPowerTable", menuName = "GameSettings/HandPowerTable")]
public class HandPowerTable : ScriptableObject
{
    [SerializeField] StackTable stack;
    [SerializeField] ChainTable chain;

    /// <summary>
    /// 役の倍率を返す
    /// </summary>
    /// <param name="type">役の種類</param>
    /// <param name="count">成立枚数</param>
    /// <returns></returns>
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

/// <summary>
/// 同値を揃えた時の役の強さ
/// </summary>
[Serializable]
public class StackTable
{
    const float Default = 1f;
    [SerializeField] float level2;
    [SerializeField] float level3;
    [SerializeField] float level4;

    /// <summary>
    /// 同値の役倍率を返す
    /// </summary>
    /// <param name="level">成立枚数</param>
    /// <returns></returns>
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

/// <summary>
/// 連続値を揃えた時の役の強さ
/// </summary>
[Serializable]
public class ChainTable : StackTable
{
    [SerializeField] float level5;

    /// <summary>
    /// 連続値の役倍率を返す
    /// </summary>
    /// <param name="level">成立枚数</param>
    /// <returns></returns>
    public override float Get(int level)
    {
        //連続値のみ成立枚数が5枚の役を出せるため、そのときの処理を書いたら他は同値の時の処理と同じ
        return level switch
        {
            5 => level5,
            _ => base.Get(level),
        };
    }
}