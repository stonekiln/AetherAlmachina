using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Modifiers;
using EditorTool.Extensions;
using EditorTool.Helpers;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(ModifierPolarity))]
    public class ModifierPolarityDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            List<NameTypePair> types = DerivedTypeNames.GetNameTypePair(typeof(ModifierPolarity)).FindSelectIndex(property, out int selectIndex, out bool isSelect);

            //このプロパティは固定値であるため選択済みの場合変更不可にする
            using (new EditorGUI.DisabledScope(isSelect))
            {
                EditorGUI.BeginChangeCheck();
                selectIndex = EditorGUI.Popup(position, label.text, selectIndex, types.Select(t => t.Name).ToArray());
                //プルダウンよりPolarityの変更が行われたか調べる
                if (EditorGUI.EndChangeCheck())
                {
                    //変更された場合新しくインスタンスを作成する
                    property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Derived);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.EndProperty();
        }
    }
}