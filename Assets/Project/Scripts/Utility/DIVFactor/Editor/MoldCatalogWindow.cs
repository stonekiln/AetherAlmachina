using DIVFactor.Mold;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AetherAlmachina.Editor.DIVFactor
{
    public sealed class MoldCatalogWindow : EditorWindow
    {
        Vector2 scroll;
        [MenuItem("DIVFactor/Mold catalog")]
        static void Open() => GetWindow<MoldCatalogWindow>("Mold catalog");
        void OnGUI()
        {
            if (GUILayout.Button("再生成（開いているシーン）")) MoldBaker.RebuildOpenScenes();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            if (AssetDatabase.IsValidFolder(MoldBaker.PrefabRoot))
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { MoldBaker.PrefabRoot }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                EditorGUILayout.ObjectField(path.Substring(MoldBaker.PrefabRoot.Length + 1), AssetDatabase.LoadAssetAtPath<GameObject>(path), typeof(GameObject), false);
            }
            EditorGUILayout.Space();
            if (AssetDatabase.IsValidFolder(MoldBaker.GeneratedRoot))
            foreach (string guid in AssetDatabase.FindAssets("t:PrefabCatalog", new[] { MoldBaker.GeneratedRoot }))
            {
                PrefabCatalog catalog = AssetDatabase.LoadAssetAtPath<PrefabCatalog>(AssetDatabase.GUIDToAssetPath(guid));
                EditorGUILayout.LabelField(catalog.name, EditorStyles.boldLabel);
                foreach (MoldEntry entry in catalog.Entries)
                {
                    EditorGUILayout.ObjectField(entry.Path, entry.Prefab, typeof(MoldMetadata), false);
                    foreach (MoldDependency dependency in catalog.Dependencies.Where(dependency => dependency.Target == entry.Path))
                        EditorGUILayout.LabelField("  " + dependency.Source + " → " + dependency.Member, EditorStyles.wordWrappedLabel);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
