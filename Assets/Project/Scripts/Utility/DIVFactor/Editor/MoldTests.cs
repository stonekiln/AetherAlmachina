using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DIVFactor.Lifetime;
using DIVFactor.Mold;
using DIVFactor.Spawner;
using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;

namespace AetherAlmachina.Editor.DIVFactor
{
    public class MoldTests
    {
        readonly List<GameObject> created = new();
        static readonly HashSet<string> known = new(StringComparer.Ordinal) { "A", "B", "Unused" };

        [TestCase("")]
        [TestCase("/A")]
        [TestCase("../A")]
        [TestCase("A//B")]
        [TestCase("A.prefab")]
        [TestCase("A ")]
        public void InvalidLogicalPathsAreRejected(string path)
            => Assert.Throws<ArgumentException>(() => MoldPath.Normalize(path));

        [Test]
        public void WindowsSeparatorsBecomeLogicalSeparators()
            => Assert.That(MoldPath.Normalize("Stage\\Debug"), Is.EqualTo("Stage/Debug"));

        [TestCase(typeof(MissingMold), "Mold属性がありません")]
        [TestCase(typeof(EmptyMold), "空にできません")]
        [TestCase(typeof(UnknownMold), "対応するPrefabがありません")]
        [TestCase(typeof(WrongOwner), "ILifetimeSpawnerが必要")]
        [TestCase(typeof(StaticMold), "インスタンスフィールド")]
        [TestCase(typeof(ReadonlyMold), "インスタンスフィールド")]
        [TestCase(typeof(RepeatedParameters), "同じ型の引数")]
        [TestCase(typeof(AttributeOnDelegate), "対応するFactoryフィールド")]
        public void InvalidFactoryFieldsHaveActionableErrors(Type componentType, string message)
            => Assert.That(Assert.Throws<BuildFailedException>(() => MoldBaker.ValidateType(componentType, known)).Message,
                Does.Contain(message));

        [Test]
        public void OldLifecycleTableIsRejectedUntilRebaked()
        {
            MoldMetadata metadata = MakeMetadata("scope");
            var format = typeof(MoldMetadata).GetField("format", BindingFlags.Instance | BindingFlags.NonPublic);
            format.SetValue(metadata, 1);
            Assert.That(metadata.IsBaked, Is.False);
            metadata.EditorBake(Array.Empty<MonoBehaviour>(), Array.Empty<FactoryBinding>());
            Assert.That(metadata.IsBaked, Is.True);
        }

        [Test]
        public void BasePrivateFieldsAreValidatedAndSaved()
        {
            Assert.DoesNotThrow(() => MoldBaker.ValidateType(typeof(InheritedMold), known));
            Assert.That(MoldBaker.Fields(typeof(InheritedMold)).Any(field => field.Name == "fromBase"), Is.True);
        }

        [Test]
        public void MultipleRootsAndCyclesCollectOnlyReachableMolds()
        {
            MoldMetadata a = MakeMetadata("A", "B");
            MoldMetadata b = MakeMetadata("B", "A");
            MoldMetadata unused = MakeMetadata("Unused");
            var prefabs = new Dictionary<string, MoldMetadata> { ["A"] = a, ["B"] = b, ["Unused"] = unused };
            Assert.That(MoldBaker.Collect(new[] { "A", "B", "A" }, prefabs).Select(entry => entry.Path), Is.EqualTo(new[] { "A", "B" }));
            Assert.That(MoldBaker.Collect(Array.Empty<string>(), prefabs), Is.Empty);
        }

        [Test]
        public void DuplicateFactoryTypesRequireAnExplicitFieldName()
        {
            GameObject root = Make("Spawner");
            SameTypeMolds spawner = root.AddComponent<SameTypeMolds>();
            MoldMetadata metadata = root.AddComponent<MoldMetadata>();
            metadata.EditorBake(new MonoBehaviour[] { spawner }, new[]
            {
                Binding(spawner, "first", "A"), Binding(spawner, "second", "B")
            });
            Assert.That(Assert.Throws<InvalidOperationException>(() => MoldTestTools.CallMetadata(metadata, spawner, typeof(ActionFactory), null)).Message,
                Does.Contain("nameof"));
            FactoryBinding selected = MoldTestTools.CallMetadata(metadata, spawner, typeof(ActionFactory), "second");
            Assert.That(selected.Path, Is.EqualTo("B"));
        }

        [Test]
        public void BakedReferencesPointAtEachCloneAndIncludeInactiveChildren()
        {
            GameObject root = Make("Mold");
            GameObject child = Make("inactive child");
            child.transform.SetParent(root.transform, false);
            child.SetActive(false);
            SameTypeMolds spawner = child.AddComponent<SameTypeMolds>();
            MoldMetadata source = root.AddComponent<MoldMetadata>();
            source.EditorBake(new MonoBehaviour[] { spawner }, new[] { Binding(spawner, "first", "A") });
            MoldMetadata clone = UnityEngine.Object.Instantiate(source);
            created.Add(clone.gameObject);
            Assert.That(clone.Components[0], Is.Not.SameAs(spawner));
            Assert.That(clone.Components[0].transform.root, Is.SameAs(clone.transform));
            Assert.That(clone.Bindings[0].Owner, Is.SameAs(clone.Components[0]));
            Assert.That(clone.Components[0].gameObject.activeSelf, Is.False);
        }

        [Test]
        public void CatalogAttachmentIsAtomicAndSurvivesOneSceneDetaching()
        {
            MoldMetadata first = MakeMetadata("first");
            MoldMetadata other = MakeMetadata("other");
            PrefabCatalog a = ScriptableObject.CreateInstance<PrefabCatalog>();
            PrefabCatalog b = ScriptableObject.CreateInstance<PrefabCatalog>();
            a.EditorSetEntries(new[] { new MoldEntry("A", first) });
            b.EditorSetEntries(new[] { new MoldEntry("B", other), new MoldEntry("A", other) });
            try
            {
                MoldTestTools.Catalog("Attach", a);
                MoldTestTools.Catalog("Attach", a);
                Assert.Throws<InvalidOperationException>(() => MoldTestTools.Catalog("Attach", b));
                Assert.Throws<InvalidOperationException>(() => global::DIVFactor.MoldObjects.Load("B"));
                MoldTestTools.Catalog("Detach", a);
                Assert.That(global::DIVFactor.MoldObjects.Load("A"), Is.SameAs(first.gameObject));
            }
            finally
            {
                MoldTestTools.Catalog("Detach", a);
                UnityEngine.Object.DestroyImmediate(a);
                UnityEngine.Object.DestroyImmediate(b);
            }
        }

        static FactoryBinding Binding(MonoBehaviour owner, string member, string path)
            => new(owner, member, typeof(ActionFactory).AssemblyQualifiedName, path);

        MoldMetadata MakeMetadata(string name, params string[] dependencies)
        {
            MoldMetadata metadata = Make(name).AddComponent<MoldMetadata>();
            metadata.EditorBake(Array.Empty<MonoBehaviour>(), dependencies.Select(path => Binding(metadata, path, path)).ToArray());
            return metadata;
        }

        GameObject Make(string name)
        {
            GameObject instance = new(name);
            created.Add(instance);
            return instance;
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (GameObject instance in created) if (instance) UnityEngine.Object.DestroyImmediate(instance);
            created.Clear();
        }

#pragma warning disable CS0169, CS0649
        public class MissingMold : MonoBehaviour, ILifetimeSpawner { ActionFactory factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class EmptyMold : MonoBehaviour, ILifetimeSpawner { [Mold("")] ActionFactory factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class UnknownMold : MonoBehaviour, ILifetimeSpawner { [Mold("Missing")] ActionFactory factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class WrongOwner : MonoBehaviour { [Mold("A")] ActionFactory factory; }
        public class StaticMold : MonoBehaviour, ILifetimeSpawner { [Mold("A")] static ActionFactory factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class ReadonlyMold : MonoBehaviour, ILifetimeSpawner { [Mold("A")] readonly ActionFactory factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class RepeatedParameters : MonoBehaviour, ILifetimeSpawner { [Mold("A")] ActionFactory<int, int> factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class AttributeOnDelegate : MonoBehaviour, ILifetimeSpawner { [Mold("A")] Action factory; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class BaseMold : MonoBehaviour, ILifetimeSpawner { [Mold("A")] ActionFactory fromBase; public void SpawnConfigure(SpawnerBuilder builder) { } }
        public class InheritedMold : BaseMold { }
        public class SameTypeMolds : MonoBehaviour, ILifetimeSpawner
        {
            [Mold("A")] ActionFactory first;
            [Mold("B")] ActionFactory second;
            public void SpawnConfigure(SpawnerBuilder builder) { }
        }
#pragma warning restore CS0169, CS0649
    }

    internal static class MoldTestTools
    {
        internal static void Call(LifetimeObject owner, string method, params object[] arguments)
            => Invoke(typeof(LifetimeObject), owner, method, arguments);
        internal static FactoryBinding CallMetadata(MoldMetadata metadata, MonoBehaviour owner, Type factoryType, string fieldName)
            => (FactoryBinding)Invoke(typeof(MoldMetadata), metadata, "Select", new object[] { owner, factoryType, fieldName });
        internal static void Catalog(string method, PrefabCatalog catalog)
            => Invoke(typeof(global::DIVFactor.MoldObjects), null, method, new object[] { catalog });
        static object Invoke(Type declaringType, object target, string method, object[] arguments)
        {
            try { return declaringType.GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Invoke(target, arguments); }
            catch (TargetInvocationException exception)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }
        }
    }
}
