using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    static class ArrayLog
    {
        /// <summary>
        /// リストの中身を表示する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        static public void Show<T>(this List<T> list)
        {
            Debug.Log(string.Join(",", list));
        }

        /// <summary>
        /// 配列の中身を表示する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        static public void Show<T>(this T[] list)
        {
            Debug.Log(string.Join(",", list));
        }
    }
}