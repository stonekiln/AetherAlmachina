using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Selectors;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(Selector))]
    public class SelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //インスタンス可能な派生クラス一覧をアルファベット順にリストとして保持する
            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(Selector)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;

            //Selectorが設定されているか
            if (property.managedReferenceValue is null)
            {
                //未設定の場合限定でNoneの選択肢を出現させる
                KeyValuePair<string, Type> none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }
            else
            {
                selectIndex = types.FindIndex(t => t.Value == property.managedReferenceValue.GetType());
            }

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(position, label.text, selectIndex, types.Select(t => t.Key).ToArray());
            //プルダウンよりSelectorの変更が行われたか調べる
            if (EditorGUI.EndChangeCheck())
            {
                //変更された場合新しくインスタンスを作成する
                property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
}