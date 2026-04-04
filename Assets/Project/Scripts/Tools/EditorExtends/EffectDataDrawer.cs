using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect;
using UnityEditor;
using UnityEngine;
using Utility;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(EffectData))]
    public class EffectDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty effectProp = property.FindPropertyRelative(BackingField.Get("Effect"));
            SerializedProperty paramProp = property.FindPropertyRelative(BackingField.Get("Parameter"));

            float height = EditorGUIUtility.singleLineHeight;

            if (effectProp.managedReferenceValue is SkillEffect)
            {
                height += EditorGUI.GetPropertyHeight(paramProp, true);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect Rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty effectProp = property.FindPropertyRelative(BackingField.Get("Effect"));
            SerializedProperty paramProp = property.FindPropertyRelative(BackingField.Get("Parameter"));

            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(SkillEffect)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;

            if (effectProp.managedReferenceValue is null)
            {
                KeyValuePair<string, Type> none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }
            else
            {
                selectIndex = types.FindIndex(t => t.Value == effectProp.managedReferenceValue.GetType());
            }

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(Rect, label.text, selectIndex, types.Select(t => t.Key).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                effectProp.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                SkillEffect effect = effectProp.managedReferenceValue as SkillEffect;

                paramProp.managedReferenceValue = Activator.CreateInstance(effect.ParameterType);

                property.serializedObject.ApplyModifiedProperties();
                paramProp = property.FindPropertyRelative(BackingField.Get("Parameter"));
            }

            if (paramProp.managedReferenceValue is not null)
            {
                Rect.y += EditorGUIUtility.singleLineHeight;
                Rect.height = EditorGUI.GetPropertyHeight(paramProp, true);
                EditorGUI.PropertyField(Rect, paramProp, true);
            }

            EditorGUI.EndProperty();
        }
    }
}