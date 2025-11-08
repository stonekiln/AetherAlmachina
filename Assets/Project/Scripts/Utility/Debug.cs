using System.Collections.Generic;
using UnityEngine;

class Show
{
    static public void Array(List<int> list)
    {
        Debug.Log(string.Join(",", list));
    }

    static public void Array(int[] list)
    {
        Debug.Log(string.Join(",", list));
    }
}