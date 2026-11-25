using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AetherAlmachina.Editor.DIVFactor
{
    // Import中にPrefabを保存せず、AssetDatabaseの処理が終了してから更新する。
    [InitializeOnLoad]
    sealed class MoldAssetPostprocessor : AssetPostprocessor
    {
        static bool scheduled;
        static MoldAssetPostprocessor() => Schedule();

        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            bool externalChange = false;
            foreach (string path in imported.Concat(deleted).Concat(moved).Concat(movedFrom))
                if (path.StartsWith(MoldBaker.PrefabRoot + "/", StringComparison.Ordinal) && path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                    externalChange |= !MoldBaker.WrittenPrefabs.Remove(path);
            if (externalChange) Schedule();
        }

        static void Schedule()
        {
            if (scheduled) return;
            scheduled = true;
            EditorApplication.delayCall += Refresh;
        }

        static void Refresh()
        {
            scheduled = false;
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            try { MoldBaker.RefreshPrefabs(); }
            catch (Exception exception) { Debug.LogException(exception); }
        }
    }
}
