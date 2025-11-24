using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
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

        static public void Show(this float[] list)
        {
            Debug.Log(string.Join(",", list));
        }
    }
}