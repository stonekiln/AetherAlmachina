using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Contracts;
using EditorTool.Extensions;
using EditorTool.Helpers;
using UnityEditor;
using UnityEngine;

namespace EditorExtends
{
    [CustomPropertyDrawer(typeof(EnchantContract))]
    public class EnchantContractDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            List<NameTypePair> types = DerivedTypeNames.GetNameTypePair(typeof(EnchantContract)).FindSelectIndex(property, out int selectIndex);

            //一行分のRectを保持する
            Rect popupRect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(popupRect, label.text, selectIndex, types.Select(t => t.Name).ToArray());
            //プルダウンよりコントラクトの変更が行われたか調べる
            if (EditorGUI.EndChangeCheck())
            {
                //変更された場合新しくインスタンスを作成する
                property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Derived);
                property.serializedObject.ApplyModifiedProperties();
            }
            //コントラクトのインスタンスが設定されている場合それを表示する
            if (property.managedReferenceValue != null)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }
    }
}