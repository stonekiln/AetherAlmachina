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
            //インスタンス可能な派生クラス一覧をアルファベット順にリストとして保持する
            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(SkillEffect)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;

            //エフェクトが設定されているか
            if (effectProp.managedReferenceValue is null)
            {
                //未設定の場合限定でNoneの選択肢を出現させる
                KeyValuePair<string, Type> none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }
            else
            {
                selectIndex = types.FindIndex(t => t.Value == effectProp.managedReferenceValue.GetType());
            }

            //一行分のRectを保持する
            Rect rect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(rect, label.text, selectIndex, types.Select(t => t.Key).ToArray());
            //プルダウンよりエフェクトの変更が行われたか調べる
            if (EditorGUI.EndChangeCheck())
            {
                //変更された場合新しくエフェクトとそのパラメータのインスタンスを作成する
                effectProp.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                SkillEffect effect = effectProp.managedReferenceValue as SkillEffect;

                paramProp.managedReferenceValue = Activator.CreateInstance(effect.ParameterType);

                property.serializedObject.ApplyModifiedProperties();
                paramProp = property.FindPropertyRelative(BackingField.Get("Parameter"));
            }
            //パラメータのインスタンスが設定されている場合それを表示する
            if (paramProp.managedReferenceValue is not null)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.height = EditorGUI.GetPropertyHeight(paramProp, true);
                EditorGUI.PropertyField(rect, paramProp, true);
            }

            EditorGUI.EndProperty();
        }
    }
}