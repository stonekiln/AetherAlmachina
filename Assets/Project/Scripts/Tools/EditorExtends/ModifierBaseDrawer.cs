using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(ModifierBase))]
    public class ModifierBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(ModifierBase)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;
            bool isSelect;

            if (isSelect = property.managedReferenceValue is null)
            {
                KeyValuePair<string, Type> none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }
            else
            {
                selectIndex = types.FindIndex(t => t.Value == property.managedReferenceValue.GetType());
            }

            using (new EditorGUI.DisabledScope(!isSelect))
            {
                EditorGUI.BeginChangeCheck();
                selectIndex = EditorGUI.Popup(position, label.text, selectIndex, types.Select(t => t.Key).ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.EndProperty();
        }
    }
}