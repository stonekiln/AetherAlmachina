using UnityEngine;

namespace Tools.Helpers
{
    public static class Probability
    {
        /// <summary>
        /// 任意の確率で試行が成功したか判定する
        /// </summary>
        /// <param name="value">成功確率</param>
        /// <returns>判定</returns>
        public static bool Try(float value)
        {
            return Mathf.Repeat(Random.value, 1f) < value;
        }
    }
}