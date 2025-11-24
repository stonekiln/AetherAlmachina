using System.Collections.Generic;

namespace Utility
{
    public static class FisherYates
    {
        public static List<SkillData> Shaffle(this List<SkillData> array)
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