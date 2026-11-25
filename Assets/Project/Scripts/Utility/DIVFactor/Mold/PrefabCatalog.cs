using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIVFactor.Mold
{
    [Serializable]
    public sealed class MoldEntry
    {
        [SerializeField] string path;
        [SerializeField] MoldMetadata prefab;
        public string Path => path;
        public MoldMetadata Prefab => prefab;
        public MoldEntry(string path, MoldMetadata prefab) { this.path = path; this.prefab = prefab; }
    }

    [Serializable]
    public sealed class MoldDependency
    {
        [SerializeField] string source;
        [SerializeField] string member;
        [SerializeField] string target;
        public string Source => source;
        public string Member => member;
        public string Target => target;
        public MoldDependency(string source, string member, string target)
        { this.source = source; this.member = member; this.target = target; }
    }

    public sealed class PrefabCatalog : ScriptableObject
    {
        [SerializeField, HideInInspector] MoldEntry[] entries = Array.Empty<MoldEntry>();
        [SerializeField, HideInInspector] MoldDependency[] dependencies = Array.Empty<MoldDependency>();
        Dictionary<string, MoldMetadata> lookup;
        public IReadOnlyList<MoldEntry> Entries => entries;
        public IReadOnlyList<MoldDependency> Dependencies => dependencies;
        public MoldMetadata Get(string path)
        {
            path = MoldPath.Normalize(path);
            if (lookup == null)
            {
                Dictionary<string, MoldMetadata> next = new(StringComparer.Ordinal);
                foreach (MoldEntry entry in entries)
                {
                    if (entry == null || !entry.Prefab) throw new InvalidOperationException("カタログのMold参照がありません。");
                    string entryPath = MoldPath.Normalize(entry.Path);
                    if (!next.TryAdd(entryPath, entry.Prefab))
                        throw new InvalidOperationException("Moldパスが重複しています: " + entry.Path);
                }
                lookup = next;
            }
            return lookup.TryGetValue(path, out MoldMetadata prefab) && prefab
                ? prefab : throw new InvalidOperationException("実行用カタログにMoldがありません: " + path);
        }
#if UNITY_EDITOR
        public void EditorSetEntries(MoldEntry[] entries, MoldDependency[] dependencies = null)
        { this.entries = entries; this.dependencies = dependencies ?? Array.Empty<MoldDependency>(); lookup = null; }
#endif
    }

    public static class MoldPath
    {
        public static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Moldパスは空にできません。", nameof(path));
            path = path.Replace('\\', '/');
            if (path != path.Trim() || path.StartsWith("/") || path.EndsWith("/") || path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Moldパスは拡張子なしの相対パスにしてください: " + path);
            foreach (string segment in path.Split('/'))
                if (segment.Length == 0 || segment == "." || segment == ".." || segment.IndexOfAny(new[] { ':', '*', '?', '"', '<', '>', '|' }) >= 0)
                    throw new ArgumentException("不正なMoldパス: " + path);
            return path;
        }
    }
}
