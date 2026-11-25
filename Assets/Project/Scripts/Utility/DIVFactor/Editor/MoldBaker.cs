using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DIVFactor.Mold;
using DIVFactor.Lifetime.Root;
using VContainer.Unity;
using DIVFactor.Spawner;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AetherAlmachina.Editor.DIVFactor
{
    public static class MoldBaker
    {
        public const string PrefabRoot = "Assets/Project/DIVFactorPrefabs";
        public const string GeneratedRoot = "Assets/Project/DIVFactorGenerated";
        internal static bool Baking { get; private set; }
        internal static readonly HashSet<string> WrittenPrefabs = new(StringComparer.Ordinal);

        public static IEnumerable<FieldInfo> Fields(Type componentType)
        {
            for (Type current = componentType; current != null && current != typeof(MonoBehaviour); current = current.BaseType)
                foreach (FieldInfo field in current.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                    yield return field;
        }

        public static void ValidateType(Type componentType, ISet<string> knownPaths)
        {
            HashSet<string> factoryNames = new(StringComparer.Ordinal);
            foreach (FieldInfo field in Fields(componentType))
            {
                bool isFactory = typeof(IMoldFactory).IsAssignableFrom(field.FieldType);
                MoldAttribute attribute = field.GetCustomAttribute<MoldAttribute>();
                if (!isFactory && attribute == null) continue;
                string location = componentType.FullName + "." + field.Name;
                if (!isFactory || field.FieldType.IsInterface || field.FieldType.IsAbstract)
                    throw new BuildFailedException(location + ": Mold属性は対応するFactoryフィールドに付けてください。");
                if (!typeof(ILifetimeSpawner).IsAssignableFrom(componentType))
                    throw new BuildFailedException(location + ": 所有コンポーネントにILifetimeSpawnerが必要です。");
                if (field.IsStatic || field.IsInitOnly || field.IsLiteral)
                    throw new BuildFailedException(location + ": Factoryは書き込み可能なインスタンスフィールドにしてください。");
                if (attribute == null) throw new BuildFailedException(location + ": Mold属性がありません。");
                Type definition = field.FieldType.IsGenericType ? field.FieldType.GetGenericTypeDefinition() : field.FieldType;
                bool returnsValue = definition == typeof(Factory<>) || definition == typeof(Factory<,>) ||
                    definition == typeof(Factory<,,>) || definition == typeof(Factory<,,,>);
                bool returnsVoid = definition == typeof(ActionFactory) || definition == typeof(ActionFactory<>) ||
                    definition == typeof(ActionFactory<,>) || definition == typeof(ActionFactory<,,>);
                if (!returnsValue && !returnsVoid) throw new BuildFailedException(location + ": 未対応のFactory型です。");
                Type[] genericArguments = field.FieldType.GetGenericArguments();
                Type[] parameters = returnsValue ? genericArguments.Take(genericArguments.Length - 1).ToArray() : genericArguments;
                if (parameters.Distinct().Count() != parameters.Length)
                    throw new BuildFailedException(location + ": 同じ型の引数は複数指定できません。意味ごとの型で包んでください。");
                if (parameters.Any(parameter => parameter == typeof(global::DIVFactor.Pooling.LifetimeArguments) || parameter == typeof(global::DIVFactor.Lifecycle.LifecycleEventHub)))
                    throw new BuildFailedException(location + ": DIVFactor管理用の型を引数にできません。");
                if (!factoryNames.Add(field.Name)) throw new BuildFailedException(location + ": 基底クラスのFactoryフィールドを同名で隠蔽できません。");
                string logicalPath;
                try { logicalPath = MoldPath.Normalize(attribute.Path); }
                catch (ArgumentException exception) { throw new BuildFailedException(location + ": " + exception.Message); }
                if (!knownPaths.Contains(logicalPath))
                    throw new BuildFailedException(location + ": 対応するPrefabがありません: " + logicalPath);
            }
        }

        static Dictionary<string, string> Index()
        {
            if (!AssetDatabase.IsValidFolder(PrefabRoot))
                throw new BuildFailedException("DIVFactor/Migrate legacy prefabs and rebuild を一度実行してください。");
            Dictionary<string, string> index = new(StringComparer.Ordinal);
            HashSet<string> portableNames = new(StringComparer.OrdinalIgnoreCase);
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabRoot }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)) continue;
                string logicalPath = assetPath.Substring(PrefabRoot.Length + 1);
                logicalPath = logicalPath.Substring(0, logicalPath.Length - ".prefab".Length);
                logicalPath = MoldPath.Normalize(logicalPath);
                if (!portableNames.Add(logicalPath)) throw new BuildFailedException("大小文字だけ異なるMoldパスがあります: " + logicalPath);
                index.Add(logicalPath, assetPath);
            }
            return index;
        }

        static void ValidateAllTypes(Dictionary<string, string> index)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:VContainerSettings"))
            {
                VContainerSettings settings = AssetDatabase.LoadAssetAtPath<VContainerSettings>(AssetDatabase.GUIDToAssetPath(guid));
                if (settings && settings.RootLifetimeScope is RootLifetimeScope)
                    throw new BuildFailedException("旧RootLifetimeScope設定があります。DIVFactor/Migrate legacy prefabs and rebuildを実行してください。");
            }
            HashSet<string> playerAssemblies = new(CompilationPipeline.GetAssemblies(AssembliesType.Player).Select(assembly => assembly.name));
            HashSet<string> paths = new(index.Keys, StringComparer.Ordinal);
            foreach (Type componentType in TypeCache.GetTypesDerivedFrom<MonoBehaviour>())
                if (!componentType.ContainsGenericParameters && playerAssemblies.Contains(componentType.Assembly.GetName().Name))
                    ValidateType(componentType, paths);
        }

        static FactoryBinding[] Bindings(MonoBehaviour[] targets, Dictionary<string, string> index)
        {
            List<FactoryBinding> bindings = new();
            HashSet<string> paths = new(index.Keys, StringComparer.Ordinal);
            foreach (MonoBehaviour component in targets)
            {
                if (!component) throw new BuildFailedException("Missing ScriptがあるためMoldを生成できません。");
                ValidateType(component.GetType(), paths);
                foreach (FieldInfo field in Fields(component.GetType()))
                {
                    MoldAttribute attribute = field.GetCustomAttribute<MoldAttribute>();
                    if (attribute != null)
                        bindings.Add(new(component, field.Name, field.FieldType.AssemblyQualifiedName, MoldPath.Normalize(attribute.Path)));
                }
            }
            return bindings.ToArray();
        }

        static MonoBehaviour[] Components(GameObject root)
            => root.GetComponentsInChildren<MonoBehaviour>(true)
                .Where(component => !(component is MoldMetadata) && !(component is MoldSceneBootstrap)).ToArray();

        static void BakePrefabs(Dictionary<string, string> index)
        {
            foreach (string assetPath in index.Values.OrderBy(path => path, StringComparer.Ordinal))
            {
                GameObject root = PrefabUtility.LoadPrefabContents(assetPath);
                try
                {
                    MonoBehaviour[] components = Components(root);
                    if (components.Any(component => component is global::DIVFactor.Lifetime.LifetimeObject))
                        throw new BuildFailedException(assetPath + ": Mold内にLifetimeObjectを直接配置しないでください。");
                    FactoryBinding[] bindings = Bindings(components, index);
                    bool added = !root.TryGetComponent(out MoldMetadata metadata);
                    if (added) metadata = root.AddComponent<MoldMetadata>();
                    string previous = EditorJsonUtility.ToJson(metadata);
                    metadata.EditorBake(components, bindings);
                    if (added || previous != EditorJsonUtility.ToJson(metadata))
                    {
                        WrittenPrefabs.Add(assetPath);
                        PrefabUtility.SaveAsPrefabAsset(root, assetPath);
                    }
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
        }

        public static MoldEntry[] Collect(IEnumerable<string> roots, IReadOnlyDictionary<string, MoldMetadata> prefabs)
        {
            HashSet<string> visited = new(StringComparer.Ordinal);
            Queue<string> pending = new(roots);
            while (pending.Count != 0)
            {
                string logicalPath = MoldPath.Normalize(pending.Dequeue());
                if (!visited.Add(logicalPath)) continue;
                if (!prefabs.TryGetValue(logicalPath, out MoldMetadata prefab) || !prefab)
                    throw new BuildFailedException("依存先Moldがありません: " + logicalPath);
                foreach (FactoryBinding binding in prefab.Bindings) pending.Enqueue(binding.Path);
            }
            return visited.OrderBy(path => path, StringComparer.Ordinal).Select(path => new MoldEntry(path, prefabs[path])).ToArray();
        }

        static string CatalogPath(Scene scene)
        {
            if (string.IsNullOrEmpty(scene.path)) throw new BuildFailedException("DIVFactorを使うシーンを保存してください。");
            return GeneratedRoot + "/Catalog_" + AssetDatabase.AssetPathToGUID(scene.path) + ".asset";
        }

        static MonoBehaviour[] SceneComponents(Scene scene)
            => scene.GetRootGameObjects().SelectMany(root => Components(root)).ToArray();

        static void StampScene(Scene scene, PrefabCatalog catalog, Dictionary<string, string> index)
        {
            MoldSceneBootstrap bootstrap = scene.GetRootGameObjects().Select(root => root.GetComponent<MoldSceneBootstrap>()).FirstOrDefault(component => component);
            if (!bootstrap)
            {
                GameObject holder = new("[DIVFactor] Scene Data");
                SceneManager.MoveGameObjectToScene(holder, scene);
                bootstrap = holder.AddComponent<MoldSceneBootstrap>();
            }
            if (!bootstrap.TryGetComponent(out MoldMetadata metadata)) metadata = bootstrap.gameObject.AddComponent<MoldMetadata>();
            string previousMetadata = EditorJsonUtility.ToJson(metadata);
            string previousBootstrap = EditorJsonUtility.ToJson(bootstrap);
            MonoBehaviour[] components = SceneComponents(scene);
            metadata.EditorBake(components, Bindings(components, index));
            bootstrap.EditorConfigure(metadata, catalog);
            if (previousMetadata != EditorJsonUtility.ToJson(metadata) || previousBootstrap != EditorJsonUtility.ToJson(bootstrap))
            {
                EditorUtility.SetDirty(metadata);
                EditorUtility.SetDirty(bootstrap);
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        static void BakeScene(Scene scene, Dictionary<string, string> index, bool stamp = true)
        {
            MonoBehaviour[] components = SceneComponents(scene);
            FactoryBinding[] bindings = Bindings(components, index);
            Dictionary<string, MoldMetadata> prefabs = index.ToDictionary(pair => pair.Key,
                pair => AssetDatabase.LoadAssetAtPath<GameObject>(pair.Value).GetComponent<MoldMetadata>(), StringComparer.Ordinal);
            MoldEntry[] entries = Collect(bindings.Select(binding => binding.Path), prefabs);
            List<MoldDependency> dependencies = new();
            foreach (FactoryBinding binding in bindings)
                dependencies.Add(new("Scene: " + scene.path, HierarchyPath(binding.Owner.transform) + ": " + binding.Owner.GetType().Name + "." + binding.FieldName, binding.Path));
            foreach (MoldEntry entry in entries)
                foreach (FactoryBinding binding in entry.Prefab.Bindings)
                    dependencies.Add(new(entry.Path, HierarchyPath(binding.Owner.transform) + ": " + binding.Owner.GetType().Name + "." + binding.FieldName, binding.Path));
            string catalogPath = CatalogPath(scene);
            PrefabCatalog catalog = AssetDatabase.LoadAssetAtPath<PrefabCatalog>(catalogPath);
            if (!catalog)
            {
                Directory.CreateDirectory(GeneratedRoot);
                AssetDatabase.Refresh();
                catalog = ScriptableObject.CreateInstance<PrefabCatalog>();
                AssetDatabase.CreateAsset(catalog, catalogPath);
            }
            string previous = EditorJsonUtility.ToJson(catalog);
            catalog.EditorSetEntries(entries, dependencies.ToArray());
            if (previous != EditorJsonUtility.ToJson(catalog)) EditorUtility.SetDirty(catalog);
            if (stamp) StampScene(scene, catalog, index);
        }

        static string HierarchyPath(Transform target)
            => target.parent ? HierarchyPath(target.parent) + "/" + target.name : target.name;

        public static void PrepareBuild(IEnumerable<string> scenePaths)
        {
            if (Baking) return;
            Baking = true;
            try
            {
                Dictionary<string, string> index = Index();
                ValidateAllTypes(index);
                BakePrefabs(index);
                foreach (string scenePath in scenePaths.Distinct())
                {
                    Scene preview = EditorSceneManager.OpenPreviewScene(scenePath);
                    try { BakeScene(preview, index, false); }
                    finally { EditorSceneManager.ClosePreviewScene(preview); }
                }
                AssetDatabase.SaveAssets();
            }
            finally { Baking = false; }
        }

        [MenuItem("DIVFactor/Rebuild open scenes")]
        public static void RebuildOpenScenes()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Editモードで実行してください。");
            if (Baking) return;
            Baking = true;
            try
            {
                Dictionary<string, string> index = Index();
                ValidateAllTypes(index);
                BakePrefabs(index);
                for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
                {
                    Scene scene = SceneManager.GetSceneAt(sceneIndex);
                    if (scene.isLoaded && !string.IsNullOrEmpty(scene.path)) BakeScene(scene, index);
                    else if (scene.isLoaded && SceneComponents(scene).Any(component => component is ILifetimeSpawner))
                        throw new BuildFailedException("DIVFactorを使うシーンを先に保存してください。");
                }
                AssetDatabase.SaveAssets();
            }
            finally { Baking = false; }
        }

        internal static void RefreshPrefabs()
        {
            if (Baking || !AssetDatabase.IsValidFolder(PrefabRoot)) return;
            Baking = true;
            try
            {
                Dictionary<string, string> index = Index();
                ValidateAllTypes(index);
                BakePrefabs(index);
                AssetDatabase.SaveAssets();
            }
            finally { Baking = false; }
        }

        public static void ProcessScene(Scene scene)
        {
            PrefabCatalog catalog = AssetDatabase.LoadAssetAtPath<PrefabCatalog>(CatalogPath(scene));
            if (!catalog) throw new BuildFailedException("先にMoldBaker.PrepareBuild(options.scenes)を実行してください: " + scene.path);
            Dictionary<string, string> index = Index();
            FactoryBinding[] roots = Bindings(SceneComponents(scene), index);
            Dictionary<string, MoldMetadata> prefabs = index.ToDictionary(pair => pair.Key,
                pair => AssetDatabase.LoadAssetAtPath<GameObject>(pair.Value).GetComponent<MoldMetadata>(), StringComparer.Ordinal);
            MoldEntry[] expected = Collect(roots.Select(binding => binding.Path), prefabs);
            if (!expected.Select(entry => entry.Path).SequenceEqual(catalog.Entries.Select(entry => entry.Path)))
                throw new BuildFailedException("シーンとカタログが一致しません。シーンを保存して再構築してください: " + scene.path);
            StampScene(scene, catalog, index);
        }


        // 外部インストーラーから呼び、シーンへの初回設定も保存して完了させる。
        public static void SetupProjectFromPackage()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            string token = null;
            for (int index = 0; index + 1 < arguments.Length; index++)
                if (arguments[index] == "-divfactorSetupToken") token = arguments[index + 1];
            if (string.IsNullOrEmpty(token)) throw new BuildFailedException("導入処理の識別子がありません。");
            string reportPath = Path.Combine("Library", "DIVFactorInstaller", "SetupResult.json");
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            PackageSetupResult result = new() { token = token, status = "failed", scenes = Array.Empty<string>() };
            try
            {
                MigrateLegacyPrefabs();
                result.scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).Distinct().ToArray();
                foreach (string scenePath in result.scenes)
                {
                    Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    RebuildOpenScenes();
                    if (!EditorSceneManager.SaveScene(scene))
                        throw new BuildFailedException("シーンを保存できません: " + scenePath);
                }
                AssetDatabase.SaveAssets();
                result.status = "success";
                Debug.Log("DIVFactorの導入とビルド対象シーンの保存が完了しました。");
            }
            catch (Exception exception) { result.message = exception.ToString(); throw; }
            finally { File.WriteAllText(reportPath, JsonUtility.ToJson(result, true), new System.Text.UTF8Encoding(false)); }
        }

        [Serializable]
        sealed class PackageSetupResult
        {
            public string token;
            public string status;
            public string message;
            public string[] scenes;
        }

        [MenuItem("DIVFactor/Migrate legacy prefabs and rebuild")]
        public static void MigrateLegacyPrefabs()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Editモードで実行してください。");
            foreach (string folder in new[] { "EntityObject", "SkillCard", "Stage" })
                if (AssetDatabase.IsValidFolder("Assets/Project/Resources/" + folder) && AssetDatabase.IsValidFolder(PrefabRoot + "/" + folder))
                    throw new BuildFailedException("移動先が既にあります。手動で統合してください: " + PrefabRoot + "/" + folder);
            if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Project/Resources/LifetimeScopes.prefab") &&
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Project/DIVFactorLegacy/LifetimeScopes.prefab"))
                throw new BuildFailedException("旧Rootの移動先が既にあります。先に重複を解消してください。");
            Directory.CreateDirectory(PrefabRoot);
            AssetDatabase.Refresh();
            foreach (string folder in new[] { "EntityObject", "SkillCard", "Stage" })
            {
                string source = "Assets/Project/Resources/" + folder;
                string destination = PrefabRoot + "/" + folder;
                if (!AssetDatabase.IsValidFolder(source)) continue;
                if (AssetDatabase.IsValidFolder(destination))
                    throw new BuildFailedException("移動先が既にあります。手動で統合してください: " + destination);
                string failure = AssetDatabase.MoveAsset(source, destination);
                if (!string.IsNullOrEmpty(failure)) throw new BuildFailedException(failure);
            }
            string oldRoot = "Assets/Project/Resources/LifetimeScopes.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(oldRoot))
            {
                Directory.CreateDirectory("Assets/Project/DIVFactorLegacy");
                AssetDatabase.Refresh();
                string failure = AssetDatabase.MoveAsset(oldRoot, "Assets/Project/DIVFactorLegacy/LifetimeScopes.prefab");
                if (!string.IsNullOrEmpty(failure)) throw new BuildFailedException(failure);
            }
            foreach (string guid in AssetDatabase.FindAssets("t:VContainerSettings"))
            {
                VContainerSettings settings = AssetDatabase.LoadAssetAtPath<VContainerSettings>(AssetDatabase.GUIDToAssetPath(guid));
                if (settings && settings.RootLifetimeScope is RootLifetimeScope)
                {
                    settings.RootLifetimeScope = null;
                    EditorUtility.SetDirty(settings);
                }
            }
            AssetDatabase.SaveAssets();
            RebuildOpenScenes();
            PrepareBuild(EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path));
            Debug.Log("DIVFactorの移行と事前生成が完了しました。変更されたシーンを保存してください。");
        }
    }

    [InitializeOnLoad]
    static class MoldPlayGuard
    {
        static MoldPlayGuard()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state != PlayModeStateChange.ExitingEditMode) return;
                try
                {
                    if (EditorSettings.enterPlayModeOptionsEnabled && (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableSceneReload) != 0)
                        throw new InvalidOperationException("DIVFactorの起動にはReload Sceneが必要です。Enter Play Mode Settingsを確認してください。");
                    MoldBaker.RebuildOpenScenes();
                }
                catch (Exception exception)
                {
                    EditorApplication.isPlaying = false;
                    Debug.LogException(exception);
                }
            };
        }
    }

    public sealed class MoldBuildGuard : IPreprocessBuildWithReport
    {
        public int callbackOrder => -10000;
        public void OnPreprocessBuild(BuildReport report)
            => MoldBaker.PrepareBuild(EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path));
    }
}
