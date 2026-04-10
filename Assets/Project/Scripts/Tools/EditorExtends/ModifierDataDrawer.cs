using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEditor;
using UnityEngine;
using Utility;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(ModifierData))]
    public class ModifierDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty modifierAssetProp = property.FindPropertyRelative("type");

            float height = EditorGUIUtility.singleLineHeight;
            if (modifierAssetProp.objectReferenceValue != null)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };


            SerializedProperty modifierAssetProp = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(rect, modifierAssetProp, label, true);

            if (modifierAssetProp.objectReferenceValue != null)
            {
                SerializedProperty valueProp = property.FindPropertyRelative(BackingField.Get("Value"));
                SerializedProperty modifierProp = new SerializedObject(modifierAssetProp.objectReferenceValue).FindProperty(BackingField.Get("ModifierType"));

                EditorGUI.BeginChangeCheck();
                if (modifierProp.managedReferenceValue is INonSliderRange nonSliderRange)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, valueProp, new GUIContent("Value" + nonSliderRange.DisplayUnit), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        valueProp.floatValue = Mathf.Clamp(valueProp.floatValue, nonSliderRange.ParameterMin, nonSliderRange.ParameterMax);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}