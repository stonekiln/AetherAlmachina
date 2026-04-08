using System;
using UnityEngine;

namespace EditorExtends.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NonSliderRangeAttribute : PropertyAttribute
    {
        public float Min { get; init; }
        public float Max { get; init; }

        public NonSliderRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}