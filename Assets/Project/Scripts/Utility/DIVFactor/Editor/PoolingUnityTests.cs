using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Lifetime;
using DIVFactor.Mold;
using DIVFactor.Pooling;
using DIVFactor.Spawner;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;

namespace AetherAlmachina.Editor.DIVFactor
{
    [Category("UnityEditor")]
    public class PoolingUnityTests
    {
        readonly List<GameObject> created = new();

        [UnityTest]
        public IEnumerator NativeScopeReusesItsContainerAndOwnerDestroysBothActiveAndWaitingChildren()
        {
            yield return new EnterPlayMode();
            PooledScope.BuildCount = 0;
            GameObject template = Make("pool template");
            template.SetActive(false);
            PooledView templateView = template.AddComponent<PooledView>();
            template.AddComponent<MoldMetadata>().EditorBake(new MonoBehaviour[] { templateView }, System.Array.Empty<FactoryBinding>());
            GameObject ownerObject = Make("pool owner");
            ownerObject.SetActive(false);
            GameObject model = Make("owner model");
            model.transform.SetParent(ownerObject.transform, false);
            PoolSpawner spawner = model.AddComponent<PoolSpawner>();
            spawner.Template = template;
            MoldMetadata ownerMetadata = model.AddComponent<MoldMetadata>();
            ownerMetadata.EditorBake(new MonoBehaviour[] { spawner }, System.Array.Empty<FactoryBinding>());
            OwnerScope owner = ownerObject.AddComponent<OwnerScope>();
            ownerObject.SetActive(true);
            Assert.That(owner.Container, Is.Null, "Awakeは自動Buildしない");
            var initialArguments = LifetimeArguments.Create();
            MoldTestTools.Call(owner, "Prepare", null, ownerMetadata, null, initialArguments, null, null, false);
            MoldTestTools.Call(owner, "BuildPrepared");
            MoldTestTools.Call(owner, "BeginUse", initialArguments);
            PooledView first = spawner.Factory(new Payload("first"));
            GameObject firstObject = first.gameObject;
            PoolingTests.FixedService fixedService = first.Fixed;
            LifecycleEventHub oldEvents = first.Events;
            first.Events.Release.OnNext(new());
            Assert.That(firstObject.activeSelf, Is.False);
            Assert.That(first.ReleasedWithClosedUsage, Is.True);
            Assert.That(first.Argument, Is.Null);
            PooledView reused = spawner.Factory(new Payload("second"));
            Assert.That(reused, Is.SameAs(first));
            Assert.That(reused.Fixed, Is.SameAs(fixedService));
            Assert.That(reused.Argument.Name, Is.EqualTo("second"));
            Assert.That(PooledScope.BuildCount, Is.EqualTo(1));
            oldEvents.Release.OnNext(new());
            Assert.That(firstObject.activeSelf, Is.True);
            model.SetActive(false);
            PooledView active = spawner.Factory(new Payload("active"));
            Assert.That(active.Argument.Name, Is.EqualTo("active"));
            model.SetActive(true);
            GameObject activeObject = active.gameObject;
            reused.Events.Release.OnNext(new());
            LifetimePool pool = model.GetComponent<LifetimePool>();
            Assert.That((pool.ActiveCount, pool.InactiveCount), Is.EqualTo((1, 1)));
            owner.Container.Resolve<LifecycleEventHub>().Destroy.OnNext(new());
            yield return null;
            Assert.That((bool)firstObject, Is.False);
            Assert.That((bool)activeObject, Is.False);
            Assert.That(fixedService.Disposed, Is.True);
        }

        GameObject Make(string name)
        {
            GameObject instance = new(name);
            created.Add(instance);
            return instance;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (GameObject instance in created) if (instance) Object.Destroy(instance);
            created.Clear();
            yield return null;
            yield return new ExitPlayMode();
        }

        public record Payload(string Name);
        public class OwnerScope : LifetimeObject
        {
            protected override void Install(ContainerInstaller installer) { }
            protected override void Register(ComponentRegister register) { }
        }
        public class PooledScope : LifetimeObject
        {
            public static int BuildCount;
            protected override void Install(ContainerInstaller installer) { BuildCount++; installer.Install<FixedInstaller>(); }
            protected override void Register(ComponentRegister register) => register.ComponentInChild<PooledView>();
        }
        public class FixedInstaller : IInstaller
        {
            public void Install(IContainerBuilder builder) => builder.Register<PoolingTests.FixedService>(VContainer.Lifetime.Singleton);
        }
        public class PoolSpawner : MonoBehaviour, ILifetimeSpawner
        {
            public GameObject Template;
            public System.Func<Payload, PooledView> Factory;
            public void SpawnConfigure(SpawnerBuilder builder) => builder.RegisterPooled<PooledScope>(Template, 1).Inject(out Factory);
        }
        public class PooledView : MonoBehaviour, IActiveComponent, IReleasable
        {
            public Payload Argument;
            public PoolingTests.FixedService Fixed;
            public LifecycleEventHub Events;
            UsageLifetime usage;
            public bool ReleasedWithClosedUsage;
            public void Injection(InjectableResolver resolver) { resolver.Inject(out Argument); resolver.Inject(out Fixed); resolver.Inject(out Events); }
            public void Subscribe(SubscriptionContext context) => usage = context.Lifetime;
            public void Release() => ReleasedWithClosedUsage = usage.IsDisposed;
        }
    }
}
