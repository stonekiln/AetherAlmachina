using System.Collections.Generic;

namespace Utility
{
    public static class FisherYates
    {
        /// <summary>
        /// リストの中身をシャッフルする
        /// </summary>
        /// <description>
        /// シャッフル方法はフィッシャーイェーツ
        /// </description>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<T> Shaffle<T>(this List<T> array)
        {
            for (int i = array.Count - 1; i > 0; i--)
            {
                int RandomNumber = UnityEngine.Random.Range(0, i);
                (array[i], array[RandomNumber]) = (array[RandomNumber], array[i]);
            }
            return array;
        }
    }
}