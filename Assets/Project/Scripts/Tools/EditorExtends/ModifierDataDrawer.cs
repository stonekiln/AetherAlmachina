using System;
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
            //Modifierの種類が指定されている場合そのパラメータを設定するためのスペースを確保する
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

            EditorGUI.BeginChangeCheck();
            SerializedProperty modifierAssetProp = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(rect, modifierAssetProp, label, true);
            //Modifierの種類が設定されているか(1項目)
            //設定されている場合必要なパラメータをキャストして取得する(2項目,3項目)
            if (modifierAssetProp.objectReferenceValue is ModifierAsset modifierAsset && modifierAsset.ModifierType is IModifierUnit Unit && modifierAsset.Polarity is ModifierPolarity polarity)
            {

                SerializedProperty valueProp = property.FindPropertyRelative(BackingField.Get("Value"));
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, valueProp, new GUIContent(polarity.DisplaySign + valueProp.displayName + Unit.DisplayUnit), true);
                if (EditorGUI.EndChangeCheck())
                {
                    valueProp.floatValue = Mathf.Clamp(valueProp.floatValue, polarity.ParameterMin, polarity.ParameterMax);
                }
            }
            else
            {
                EditorGUI.EndChangeCheck();
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}