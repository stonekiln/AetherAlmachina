using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(EffectData))]
    public class EffectDataDrawer : PropertyDrawer
    {
        private const float VerticalSpacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty effectProp = property.FindPropertyRelative("<Effect>k__BackingField");
            SerializedProperty paramProp = property.FindPropertyRelative("<Parameter>k__BackingField");

            float height = EditorGUIUtility.singleLineHeight;

            if (effectProp.managedReferenceValue is SkillEffect)
            {
                height += VerticalSpacing + EditorGUI.GetPropertyHeight(paramProp, true);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect Rect = new(
                position.x,
                position.y,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            SerializedProperty effectProp = property.FindPropertyRelative("<Effect>k__BackingField");
            SerializedProperty paramProp = property.FindPropertyRelative("<Parameter>k__BackingField");

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
                paramProp = property.FindPropertyRelative("<Parameter>k__BackingField");
            }

            Rect.y += EditorGUIUtility.singleLineHeight + VerticalSpacing;
            Rect.height = EditorGUI.GetPropertyHeight(paramProp, true);

            EditorGUI.PropertyField(Rect, paramProp, true);

            EditorGUI.EndProperty();
        }
    }
}