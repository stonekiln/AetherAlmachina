using DIVFactor.Mold;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIVFactor
{
    /// <summary>読み込まれたシーンのカタログを統合する。Loadは参照取得であり、Instantiateは行わない。</summary>
    public static class MoldObjects
    {
        static readonly List<PrefabCatalog> catalogs = new();
        static readonly Dictionary<string, MoldMetadata> lookup = new(StringComparer.Ordinal);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() { catalogs.Clear(); lookup.Clear(); }

        internal static void Attach(PrefabCatalog catalog)
        {
            if (!catalog) throw new ArgumentNullException(nameof(catalog));
            // 検証が失敗したときに、既存シーンの参照表を途中まで変更しない。
            Dictionary<string, MoldMetadata> additions = new(StringComparer.Ordinal);
            foreach (MoldEntry entry in catalog.Entries)
            {
                string path = MoldPath.Normalize(entry.Path);
                if (!entry.Prefab) throw new InvalidOperationException("Mold参照がありません: " + path);
                if ((lookup.TryGetValue(path, out MoldMetadata existing) && existing != entry.Prefab) ||
                    (additions.TryGetValue(path, out existing) && existing != entry.Prefab))
                    throw new InvalidOperationException("シーン間でMoldパスが衝突しています: " + path);
                additions[path] = entry.Prefab;
            }
            foreach (var pair in additions) lookup[pair.Key] = pair.Value;
            catalogs.Add(catalog);
        }

        internal static void Detach(PrefabCatalog catalog)
        {
            if (!catalogs.Remove(catalog)) return;
            lookup.Clear();
            foreach (PrefabCatalog remaining in catalogs)
                foreach (MoldEntry entry in remaining.Entries) lookup[entry.Path] = entry.Prefab;
        }

        public static GameObject Load(string path)
        {
            path = MoldPath.Normalize(path);
            return lookup.TryGetValue(path, out MoldMetadata prefab) && prefab
                ? prefab.gameObject : throw new InvalidOperationException("Moldが登録されていません: " + path);
        }
    }
}
