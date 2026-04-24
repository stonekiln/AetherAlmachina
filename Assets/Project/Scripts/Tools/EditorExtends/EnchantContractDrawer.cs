using System;
using System.Collections.Generic;
using System.Linq;
using AetherAlmachina.Skill.Effect.Contracts;
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
            //インスタンス可能な派生クラス一覧をアルファベット順にリストとして保持する
            List<KeyValuePair<string, Type>> types = TypeCache.GetTypesDerivedFrom(typeof(EnchantContract)).Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).Select(t => new KeyValuePair<string, Type>(t.Name, t)).ToList();
            int selectIndex;

            //コントラクトが設定されているか
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

            //一行分のRectを保持する
            Rect popupRect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.BeginChangeCheck();
            selectIndex = EditorGUI.Popup(popupRect, label.text, selectIndex, types.Select(t => t.Key).ToArray());
            //プルダウンよりコントラクトの変更が行われたか調べる
            if (EditorGUI.EndChangeCheck())
            {
                //変更された場合新しくインスタンスを作成する
                property.managedReferenceValue = Activator.CreateInstance(types[selectIndex].Value);
                property.serializedObject.ApplyModifiedProperties();
            }
            //コントラクトのインスタンスが設定されている場合それを表示する
            if (property.managedReferenceValue is not null)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }
    }
}