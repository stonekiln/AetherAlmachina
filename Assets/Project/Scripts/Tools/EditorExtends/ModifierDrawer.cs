using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Modifiers;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(Modifier))]
    public class ModifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(Modifier)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;

            if (property.managedReferenceValue is null)
            {
                KeyValuePair<string, Type> none = new("None", null);
                types.Insert(0, none);
                EditorGUI.BeginChangeCheck();
                selectIndex = EditorGUI.Popup(position, label.text, 0, types.Select(t => t.Key).ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.LabelField(EditorGUI.PrefixLabel(position, label), property.managedReferenceValue.GetType().Name);
            }

            EditorGUI.EndProperty();
        }
    }
}