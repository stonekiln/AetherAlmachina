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
                SerializedObject modifierAssetObj = new(modifierAssetProp.objectReferenceValue);
                SerializedProperty modifierProp = modifierAssetObj.FindProperty(BackingField.Get("ModifierType"));
                SerializedProperty polarityProp = modifierAssetObj.FindProperty(BackingField.Get("Polarity"));

                EditorGUI.BeginChangeCheck();
                if (modifierProp.managedReferenceValue is IModifierUnit Unit && polarityProp.managedReferenceValue is ModifierPolarity polarity)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, valueProp, new GUIContent(polarity.DisplaySign + "Value" + Unit.DisplayUnit), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        valueProp.floatValue = Mathf.Clamp(valueProp.floatValue, polarity.ParameterMin, polarity.ParameterMax);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}