using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Lifetime;
using DIVFactor.Mold;
using DIVFactor.Pooling;
using NUnit.Framework;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

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
            MoldMetadata metadata = model.AddComponent<MoldMetadata>();
            metadata.EditorBake(new MonoBehaviour[] { probe }, System.Array.Empty<FactoryBinding>());
            NativeScope scope = scopeObject.AddComponent<NativeScope>();
            scopeObject.SetActive(true);
            var arguments = LifetimeArguments.Create();
            MoldTestTools.Call(scope, "Prepare", null, metadata, null, arguments, null, null, false);
            MoldTestTools.Call(scope, "BuildPrepared");
            MoldTestTools.Call(scope, "BeginUse", arguments);
            Assert.That(scope.Container, Is.Not.Null);
            Assert.That(view.transform.parent, Is.SameAs(destination.transform));
            Assert.That(view.activeSelf, Is.True);
            Assert.That(probe.Trace, Is.EqualTo(new[] { "inject", "initialize", "view", "updated", "subscribe", "activate", "enable" }));
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

        public class NativeProbe : MonoBehaviour, ISubscribingViewUpdater, IReleasable
        {
            LifetimeOwnership ownership;
            LifecycleEventHub events;
            UsageLifetime usage;
            public Transform Destination;
            public readonly List<string> Trace = new();
            public bool ReleasedAfterUnsubscribe;
            public void Injection(InjectableResolver resolver)
            {
                resolver.Inject(out ownership);
                resolver.Inject(out events);
                Trace.Add("inject");
                resolver.Injected.SubscribeLifecycle(() => Trace.Add("initialize"), resolver.Fail);
            }
            public void UpdateView(ViewContext context)
            {
                ownership.Track(gameObject);
                transform.SetParent(Destination, false);
                Trace.Add("view");
                context.UpdatedView.SubscribeLifecycle(() => Trace.Add("updated"), context.Fail);
            }
            public void Subscribe(SubscriptionContext context)
            {
                usage = context.Lifetime;
                Trace.Add("subscribe");
                context.Subscribed.SubscribeLifecycle(() => Trace.Add("activate"), context.Fail);
            }
            void OnEnable() => Trace.Add("enable");
            public void Release()
            {
                using System.IDisposable stillAlive = events.Release.Subscribe(_ => { });
                ReleasedAfterUnsubscribe = usage.IsDisposed;
                Trace.Add("release");
            }
        }
    }
}
