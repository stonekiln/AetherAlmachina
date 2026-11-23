using System.Collections;
using System.Collections.Generic;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Lifetime;
using NUnit.Framework;
using R3;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;
using IInitializable = DIVFactor.Lifecycle.IInitializable;

namespace AetherAlmachina.Editor.DIVFactor
{
    [Category("UnityEditor")]
    public class LifecycleUnityTests
    {
        readonly List<GameObject> created = new();

        [UnityTest]
        public IEnumerator ReparentedViewStaysOwnedAndReleaseRunsBeforeContainerDisposal()
        {
            yield return new EnterPlayMode();
            GameObject scopeObject = Make("Lifecycle test scope");
            scopeObject.SetActive(false);
            GameObject model = Make("Model");
            model.SetActive(false);
            model.transform.SetParent(scopeObject.transform, false);
            GameObject view = Make("View");
            view.transform.SetParent(model.transform, false);
            GameObject destination = Make("External view parent");
            NativeProbe probe = view.AddComponent<NativeProbe>();
            probe.Destination = destination.transform;
            NativeScope scope;
            // テストではプロジェクトの既定Rootを親として生成しない。
            using (LifetimeScope.EnqueueParent((LifetimeScope)null))
            {
                scope = scopeObject.AddComponent<NativeScope>();
                scopeObject.SetActive(true);
            }
            Assert.That(scope.Container, Is.Not.Null);
            Assert.That(view.transform.parent, Is.SameAs(destination.transform));
            Assert.That(view.activeSelf, Is.True);
            Assert.That(probe.Trace, Is.EqualTo(new[] { "inject", "initialize", "view", "subscribe", "activate" }));
            scope.Container.Resolve<LifecycleEventHub>().Release.OnNext(new());
            yield return null;
            Assert.That(probe.ReleasedAfterUnsubscribe, Is.True);
            Assert.That((bool)scopeObject, Is.False);
            Assert.That((bool)model, Is.False);
            Assert.That((bool)view, Is.False);
        }

        GameObject Make(string name)
        {
            GameObject createdObject = new(name);
            created.Add(createdObject);
            return createdObject;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (GameObject createdObject in created) if (createdObject) Object.Destroy(createdObject);
            created.Clear();
            yield return null;
            yield return new ExitPlayMode();
        }

        public class NativeScope : LifetimeObject
        {
            protected override void Install(ContainerInstaller installer) { }
            protected override void Register(ComponentRegister register) { }
        }

        public class NativeProbe : MonoBehaviour, IActiveComponent, IReactiveView, IReleasable
        {
            LifetimeOwnership ownership;
            LifecycleEventHub events;
            UsageLifetime usage;
            public Transform Destination;
            public readonly List<string> Trace = new();
            public bool ReleasedAfterUnsubscribe;
            public void Injection(InjectableResolver resolver) { resolver.Inject(out ownership); resolver.Inject(out events); Trace.Add("inject"); }
            public void Initialize() => Trace.Add("initialize");
            public void UpdateView() { ownership.Track(gameObject); transform.SetParent(Destination, false); Trace.Add("view"); }
            public void Subscribe(UsageLifetime lifetime) { usage = lifetime; Trace.Add("subscribe"); }
            public void Activate() => Trace.Add("activate");
            public void Release()
            {
                using System.IDisposable stillAlive = events.Release.Subscribe(_ => { });
                ReleasedAfterUnsubscribe = usage.IsDisposed;
                Trace.Add("release");
            }
        }
    }
}
