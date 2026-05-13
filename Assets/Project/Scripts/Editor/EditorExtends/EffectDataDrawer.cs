using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect;
using UnityEditor;
using UnityEngine;
using EditorTool.Helpers;
using EditorTool.Extensions;

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
            //エフェクトの種類が指定されている場合そのパラメータを設定するためのスペースを確保する
            if (effectProp.managedReferenceValue is SkillEffect)
            {
                height += EditorGUI.GetPropertyHeight(paramProp, true);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty effectProp = property.FindPropertyRelative(BackingField.Get("Effect"));
            SerializedProperty paramProp = property.FindPropertyRelative(BackingField.Get("Parameter"));

            List<NameTypePair> types = DerivedTypeNames.GetNameTypePair(typeof(SkillEffect)).FindSelectIndex(effectProp, out int selectIndex);

            //一行分のRectを保持する
            Rect rect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(rect, label.text, selectIndex, types.Select(t => t.Name).ToArray());
            //プルダウンよりエフェクトの変更が行われたか調べる
            if (EditorGUI.EndChangeCheck())
            {
                //変更された場合新しくエフェクトとそのパラメータのインスタンスを作成する
                effectProp.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Derived);
                SkillEffect effect = effectProp.managedReferenceValue as SkillEffect;

                paramProp.managedReferenceValue = Activator.CreateInstance(effect.ParameterType);

                property.serializedObject.ApplyModifiedProperties();
                paramProp = property.FindPropertyRelative(BackingField.Get("Parameter"));
            }
            //パラメータのインスタンスが設定されている場合それを表示する
            if (paramProp.managedReferenceValue != null)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.height = EditorGUI.GetPropertyHeight(paramProp, true);
                EditorGUI.PropertyField(rect, paramProp, true);
            }

            EditorGUI.EndProperty();
        }
    }
}
