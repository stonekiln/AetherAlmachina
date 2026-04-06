using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Contracts;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(EnchantContract))]
    public class EnchantContractDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(EnchantContract)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;

            if (property.managedReferenceValue is null)
            {
                KeyValuePair<string, Type> none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }
            else
            {
                selectIndex = types.FindIndex(t => t.Value == property.managedReferenceValue.GetType());
            }

            EditorGUI.BeginChangeCheck();
            Rect popupRect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            selectIndex = EditorGUI.Popup(popupRect, label.text, selectIndex, types.Select(t => t.Key).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                property.serializedObject.ApplyModifiedProperties();
            }

            if (property.managedReferenceValue is not null)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }
    }
}