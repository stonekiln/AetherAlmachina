using EditorExtends.Attribute;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NonSliderRangeAttribute))]
public class NonSliderRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        NonSliderRangeAttribute range = (NonSliderRangeAttribute)attribute;

        EditorGUI.PropertyField(position, property, label);

        switch (property.propertyType)
        {
            case SerializedPropertyType.Float:
                property.floatValue = Mathf.Clamp(property.floatValue, range.Min, range.Max);
                break;
            case SerializedPropertyType.Integer:
                property.intValue = Mathf.Clamp(property.intValue, (int)range.Min, (int)range.Max);
                break;
        }
    }
}