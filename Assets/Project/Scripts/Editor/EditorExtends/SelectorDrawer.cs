using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Selectors;
using EditorTool.Extensions;
using EditorTool.Helpers;
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

            List<NameTypePair> types = DerivedTypeNames.GetNameTypePair(typeof(Selector)).FindSelectIndex(property, out int selectIndex);

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(position, label.text, selectIndex, types.Select(t => t.Name).ToArray());
            //プルダウンよりSelectorの変更が行われたか調べる
            if (EditorGUI.EndChangeCheck())
            {
                //変更された場合新しくインスタンスを作成する
                property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Derived);
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }
    }
}