using System;
using System.Collections;
using System.Collections.Generic;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Lifetime;
using DIVFactor.Mold;
using DIVFactor.Pooling;
using DIVFactor.Spawner;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    [Category("UnityEditor")]
    public class MoldUnityTests
    {
        readonly List<GameObject> created = new();
        PrefabCatalog catalog;

        [UnityTest]
        public IEnumerator SameTypeFactoriesSelectTheirMoldAndDifferentOwnersKeepSeparatePools()
        {
            yield return new EnterPlayMode();
            LeafScope.Builds = 0;
            MoldMetadata firstMold = Mold("first mold");
            MoldMetadata secondMold = Mold("second mold");
            catalog = ScriptableObject.CreateInstance<PrefabCatalog>();
            catalog.EditorSetEntries(new[] { new MoldEntry("LeafA", firstMold), new MoldEntry("LeafB", secondMold) });
            (OwnerScope firstOwner, DualSpawner first) = Owner("owner one");
            (OwnerScope secondOwner, DualSpawner second) = Owner("owner two");
            LeafView a = first.First.Spawn(new Payload("one"));
            LeafView b = first.Second.Spawn(new Payload("two"));
            LeafView otherOwner = second.First.Spawn(new Payload("other owner"));
            Assert.That(a.name, Does.StartWith("first mold"));
            Assert.That(b.name, Does.StartWith("second mold"));
            Assert.That(otherOwner, Is.Not.SameAs(a));
            Assert.That(a.Argument.Name, Is.EqualTo("one"));
            Assert.That(firstOwner.GetComponentsInChildren<LeafScope>(true).Length, Is.EqualTo(2));
            Assert.That(secondOwner.GetComponentsInChildren<LeafScope>(true).Length, Is.EqualTo(1));
            foreach (LeafScope child in firstOwner.GetComponentsInChildren<LeafScope>(true))
                Assert.That(child.Parent, Is.SameAs(firstOwner));
            Assert.That(firstMold.Components[0].gameObject, Is.SameAs(firstMold.gameObject));
            a.Events.Release.OnNext(new());
            LeafView reused = first.First.Spawn(new Payload("reused"));
            Assert.That(reused, Is.SameAs(a));
            Assert.That(reused.Argument.Name, Is.EqualTo("reused"));
            Assert.That(LeafScope.Builds, Is.EqualTo(3));
            firstOwner.Container.Resolve<LifecycleEventHub>().Destroy.OnNext(new());
            yield return null;
            Assert.That((bool)a, Is.False);
            Assert.That((bool)b, Is.False);
            Assert.That((bool)otherOwner, Is.True);
        }

        MoldMetadata Mold(string name)
        {
            GameObject root = Make(name);
            root.SetActive(false);
            LeafView view = root.AddComponent<LeafView>();
            MoldMetadata metadata = root.AddComponent<MoldMetadata>();
            metadata.EditorBake(new MonoBehaviour[] { view }, Array.Empty<FactoryBinding>());
            return metadata;
        }

        (OwnerScope, DualSpawner) Owner(string name)
        {
            GameObject root = Make(name);
            DualSpawner spawner = root.AddComponent<DualSpawner>();
            MoldMetadata metadata = root.AddComponent<MoldMetadata>();
            metadata.EditorBake(new MonoBehaviour[] { spawner }, new[]
            {
                new FactoryBinding(spawner, "first", typeof(Factory<Payload, LeafView>).AssemblyQualifiedName, "LeafA"),
                new FactoryBinding(spawner, "second", typeof(Factory<Payload, LeafView>).AssemblyQualifiedName, "LeafB")
            });
            OwnerScope scope = root.AddComponent<OwnerScope>();
            Assert.That(scope.Container, Is.Null, "AddComponent直後に自動Buildしない");
            var arguments = LifetimeArguments.Create();
            MoldTestTools.Call(scope, "Prepare", null, metadata, catalog, arguments, null, null, true);
            MoldTestTools.Call(scope, "BuildPrepared");
            MoldTestTools.Call(scope, "BeginUse", arguments);
            return (scope, spawner);
        }

        GameObject Make(string name)
        {
            GameObject instance = new(name);
            created.Add(instance);
            return instance;
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            foreach (GameObject instance in created) if (instance) UnityEngine.Object.Destroy(instance);
            created.Clear();
            if (catalog) UnityEngine.Object.Destroy(catalog);
            yield return null;
            yield return new ExitPlayMode();
        }

        public record Payload(string Name);
        public class OwnerScope : LifetimeObject
        {
            protected override void Install(ContainerInstaller installer) { }
            protected override void Register(ComponentRegister register) { }
        }
        public class LeafScope : LifetimeObject
        {
            public static int Builds;
            protected override void Install(ContainerInstaller installer) => Builds++;
            protected override void Register(ComponentRegister register) => register.ComponentInChild<LeafView>();
        }
        public class LeafView : MonoBehaviour, IInjectable
        {
            public Payload Argument;
            public LifecycleEventHub Events;
            public void Injection(InjectableResolver resolver) { resolver.Inject(out Argument); resolver.Inject(out Events); }
        }
        public class DualSpawner : MonoBehaviour, ILifetimeSpawner
        {
            [Mold("LeafA")] Factory<Payload, LeafView> first;
            [Mold("LeafB")] Factory<Payload, LeafView> second;
            public Factory<Payload, LeafView> First => first;
            public Factory<Payload, LeafView> Second => second;
            public void SpawnConfigure(SpawnerBuilder builder)
            {
                builder.RegisterPooled<LeafScope>().Inject(out first, nameof(first));
                builder.RegisterPooled<LeafScope>().Inject(out second, nameof(second));
            }
        }
    }
}
