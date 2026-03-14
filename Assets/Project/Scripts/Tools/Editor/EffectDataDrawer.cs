using System;
using Skill;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class EffectDataDrawer : PropertyDrawer
{
    private const float VerticalSpacing = 2f;
    SerializedProperty effectProp;
    SerializedProperty parameterProp;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        effectProp = property.FindPropertyRelative("<Effect>k__BackingField");
        parameterProp = property.FindPropertyRelative("<Parameter>k__BackingField");

        SkillEffect effect = effectProp.objectReferenceValue as SkillEffect;
        float height = EditorGUIUtility.singleLineHeight;

        if (effect != null)
        {
            height += VerticalSpacing + EditorGUI.GetPropertyHeight(parameterProp, true);
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect currentRect = new(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        // effect を表示
        EditorGUI.PropertyField(currentRect, effectProp);

        SkillEffect effect = effectProp.objectReferenceValue as SkillEffect;

        if (effect == null)
        {
            // effect 未設定なら parameter を消しておく
            if (parameterProp.managedReferenceValue != null)
            {
                parameterProp.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        else
        {
            object currentParameter = parameterProp.managedReferenceValue;
            // 型が違う、または null の場合は作り直す
            if (currentParameter == null || currentParameter.GetType() != effect.ParameterType)
            {
                parameterProp.managedReferenceValue = Activator.CreateInstance(effect.ParameterType);
                property.serializedObject.ApplyModifiedProperties();
            }

            // 最新状態を取り直す
            parameterProp = property.FindPropertyRelative("<Parameter>k__BackingField");

            currentRect.y += EditorGUIUtility.singleLineHeight + VerticalSpacing;
            currentRect.height = EditorGUI.GetPropertyHeight(parameterProp, true);

            EditorGUI.PropertyField(currentRect, parameterProp, true);
        }
        EditorGUI.EndProperty();
    }
}