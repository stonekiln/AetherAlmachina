using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEditor;
using UnityEngine;
using Utility;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(ModifierEnchantData))]
    public class ModifierDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty modifierAssetProp = property.FindPropertyRelative(BackingField.Get("Type"));

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
            SerializedProperty modifierAssetProp = property.FindPropertyRelative(BackingField.Get("Type"));
            EditorGUI.PropertyField(rect, modifierAssetProp, label, true);
            //Modifierの種類が設定されているか(1項目)
            //設定されている場合必要なパラメータをキャストして取得する(2項目,3項目)
            if (modifierAssetProp.objectReferenceValue is ModifierAsset modifierAsset)
            {
                SerializedProperty valueProp = property.FindPropertyRelative(BackingField.Get("Value"));
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, valueProp, new GUIContent(modifierAsset.Polarity.DisplaySign + valueProp.displayName + modifierAsset.ModifierType.DisplayUnit), true);
                if (EditorGUI.EndChangeCheck())
                {
                    valueProp.floatValue = Mathf.Clamp(valueProp.floatValue, 0, float.PositiveInfinity);
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