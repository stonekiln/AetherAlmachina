using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EditorTool.Helpers
{
    /// <summary>
    /// 派生型の型と型の名前を一対一対応させる
    /// </summary>
    /// <param name="Name">型名</param>
    /// <param name="Derived">種類</param>
    public record NameTypePair(string Name, Type Derived);

    public static class DerivedTypeNames
    {
        /// <summary>
        /// 基底クラスから派生したインスタンス可能な型と名前のペアのデータのリストを返す
        /// </summary>
        /// <param name="baseType">検索する基底クラス</param>
        /// <returns></returns>
        public static List<NameTypePair> GetNameTypePair(Type baseType)
        {
            return TypeCache.GetTypesDerivedFrom(baseType).Where(t => !t.IsAbstract && !t.IsGenericType)
                    .OrderBy(t => t.Name).Select(t => new NameTypePair(t.Name, t)).ToList();
        }
    }
}