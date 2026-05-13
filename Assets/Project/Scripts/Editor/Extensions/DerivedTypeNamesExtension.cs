using System.Collections.Generic;
using EditorTool.Helpers;
using UnityEditor;

namespace EditorTool.Extensions
{
    public static class DerivedTypeNamesExtension
    {
        public static List<NameTypePair> FindSelectIndex(this List<NameTypePair> types, SerializedProperty property, out int selectIndex)
        {
            //プロパティが設定されているか
            if (property.managedReferenceValue == null)
            {
                //未設定の場合限定でNoneの選択肢を出現させる
                NameTypePair none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }
            else
            {
                //選択済みの場合は当てはまる値にインデックスを指定する
                selectIndex = types.FindIndex(t => t.Derived == property.managedReferenceValue.GetType());
            }

            return types;
        }

        public static List<NameTypePair> FindSelectIndex(this List<NameTypePair> types, SerializedProperty property, out int selectIndex, out bool isSelect)
        {
            //プロパティが設定されているか
            if (isSelect = property.managedReferenceValue != null)
            {
                //選択済みの場合は当てはまる値にインデックスを指定する
                selectIndex = types.FindIndex(t => t.Derived == property.managedReferenceValue.GetType());
            }
            else
            {
                //未設定の場合限定でNoneの選択肢を出現させる
                NameTypePair none = new("None", null);
                types.Insert(0, none);
                selectIndex = 0;
            }

            return types;
        }
    }
}