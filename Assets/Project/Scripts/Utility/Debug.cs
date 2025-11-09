using System.Collections.Generic;
using UnityEngine;

static class ArrayLog
{
    static public void Show(this List<int> list)
    {
        Debug.Log(string.Join(",", list));
    }

    static public void Show(this int[] list)
    {
        Debug.Log(string.Join(",", list));
    }
}