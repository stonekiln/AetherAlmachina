using System;
using System.Linq;
using System.Xml.Linq;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
using NUnit.Framework;
using R3;
using UnityEditor;
using UnityEditor.UnityLinker;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    public class EventHubRegistrationTests
    {
        [Test]
        public void EntityInstallerRegistersTheWholeInteractionTree()
        {
            ContainerBuilder builder = new();
            new CommandEventInstaller().Install(builder);
            Assert.That(builder.Count, Is.EqualTo(18));

            using IObjectResolver container = builder.Build();
            InteractionEventHub interaction = container.Resolve<InteractionEventHub>();
            Assert.That(interaction.LockOn, Is.SameAs(container.Resolve<LockOnEventHub>().Request));
            Assert.That(interaction.ResourceUpdate.HP.Request,
                Is.SameAs(container.Resolve<EventPort<HPUpdateRequestEvent>>()));
            Assert.That(interaction.ResourceUpdate.Cost.Response,
                Is.SameAs(container.Resolve<EventPort<CostUpdateResponseEvent>>()));
        }

        [Test]
        public void NestedHubsShareTheSamePortAndDeliverEvents()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<RootHub>();
            Assert.That(builder.Count, Is.EqualTo(6));

            using IObjectResolver container = builder.Build();
            RootHub root = container.Resolve<RootHub>();
            Assert.That(root.Left.Shared, Is.SameAs(root.Right.Shared));
            Assert.That(root.Left, Is.SameAs(container.Resolve<LeftHub>()));
            int received = 0;
            using IDisposable subscription = root.Right.Shared.Subscribe(message => received = message.Value);
            container.Resolve<EventPort<SharedEvent>>().OnNext(new(42));
            Assert.That(received, Is.EqualTo(42));
        }

        [Test]
        public void HubAndIndividualRegistrationAreIdempotentInEitherOrder()
        {
            ContainerBuilder builder = new();
            RegistrationBuilder shared = builder.RegisterEvent<SharedEvent>();
            RegistrationBuilder root = builder.RegisterEventHub<RootHub>();
            Assert.That(builder.RegisterEventHub<RootHub>(), Is.SameAs(root));
            Assert.That(builder.RegisterEvent<SharedEvent>(), Is.SameAs(shared));
            builder.RegisterEventHub<LeftHub>();
            Assert.That(builder.Count, Is.EqualTo(6));

            ContainerBuilder other = new();
            other.RegisterEventHub<RootHub>();
            other.RegisterEvent<SharedEvent>();
            Assert.That(other.Count, Is.EqualTo(6));
        }

        [Test]
        public void DifferentContainersDoNotShareRegistrationStateOrPorts()
        {
            ContainerBuilder first = new();
            ContainerBuilder second = new();
            first.RegisterEventHub<RootHub>();
            second.RegisterEventHub<RootHub>();
            using IObjectResolver firstContainer = first.Build();
            using IObjectResolver secondContainer = second.Build();
            Assert.That(second.Count, Is.EqualTo(first.Count));
            Assert.That(firstContainer.Resolve<EventPort<SharedEvent>>(),
                Is.Not.SameAs(secondContainer.Resolve<EventPort<SharedEvent>>()));
        }

        [Test]
        public void ParentScopeRegistrationDoesNotSuppressChildScopeRegistration()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<RootHub>();
            using IObjectResolver parent = builder.Build();
            using IScopedObjectResolver child = parent.CreateScope(childBuilder => childBuilder.RegisterEventHub<RootHub>());
            Assert.That(child.Resolve<EventPort<SharedEvent>>(), Is.Not.SameAs(parent.Resolve<EventPort<SharedEvent>>()));
        }

        [Test]
        public void LifetimeConflictWithAnExistingPortDoesNotPartiallyRegisterHub()
        {
            ContainerBuilder builder = new();
            builder.RegisterEvent<RightEvent>(Lifetime.Scoped);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => builder.RegisterEventHub<RootHub>());
            Assert.That(exception.Message, Does.Contain(nameof(RightEvent)).And.Contain("Scoped").And.Contain("Singleton"));
            Assert.That(builder.Count, Is.EqualTo(1));
        }

        [Test]
        public void IndividualRegistrationRejectsConflictingLifetimeAfterHub()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<RootHub>();
            Assert.Throws<InvalidOperationException>(() => builder.RegisterEvent<SharedEvent>(Lifetime.Transient));
            Assert.That(builder.Count, Is.EqualTo(6));
        }

        [Test]
        public void ConflictingHubsAreRejectedBeforeAnyRegistration()
        {
            ContainerBuilder builder = new();
            Assert.Throws<InvalidOperationException>(() => builder.RegisterEventHub<ConflictingRoot>());
            Assert.That(builder.Count, Is.Zero);
        }

        [Test]
        public void ChildHubUsesItsOwnLifetime()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<TransientRoot>();
            using IObjectResolver container = builder.Build();
            TransientRoot first = container.Resolve<TransientRoot>();
            TransientRoot second = container.Resolve<TransientRoot>();
            Assert.That(first, Is.Not.SameAs(second));
            Assert.That(first.Child, Is.SameAs(second.Child));
            Assert.That(first.Direct, Is.Not.SameAs(second.Direct));
        }

        [Test]
        public void ClosedGenericHubsKeepTheirOwnEventTypes()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<GenericHub<SharedEvent, LeftEvent>>();
            builder.RegisterEventHub<GenericHub<SharedEvent, RightEvent>>();
            using IObjectResolver container = builder.Build();
            var left = container.Resolve<GenericHub<SharedEvent, LeftEvent>>();
            var right = container.Resolve<GenericHub<SharedEvent, RightEvent>>();
            Assert.That(left.First, Is.SameAs(right.First));
            Assert.That(left.Second, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(right.Second, Is.SameAs(container.Resolve<EventPort<RightEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(5));
        }

        [Test]
        public void EventExchangerInjectsBothPortsAndExchangesEvents()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<EventExchanger<ExchangeRequest, ExchangeResponse>>();
            using IObjectResolver container = builder.Build();
            var exchanger = container.Resolve<EventExchanger<ExchangeRequest, ExchangeResponse>>();
            Assert.That(exchanger.Request, Is.SameAs(container.Resolve<EventPort<ExchangeRequest>>()));
            Assert.That(exchanger.Response, Is.SameAs(container.Resolve<EventPort<ExchangeResponse>>()));
            int received = 0;
            using IDisposable response = exchanger.Response.Subscribe(message => received = message.Value);
            using IDisposable exchange = exchanger.AsObservable().Subscribe(request => new(request.Value * 2));
            exchanger.Request.OnNext(new(21));
            Assert.That(received, Is.EqualTo(42));
            Assert.That(builder.Count, Is.EqualTo(3));
        }

        [Test]
        public void DerivedExchangerUsesInheritedPrivateInjectMethod()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<DeckDrawEventHub>();
            using IObjectResolver container = builder.Build();
            DeckDrawEventHub deck = container.Resolve<DeckDrawEventHub>();
            Assert.That(deck.Request, Is.SameAs(container.Resolve<EventPort<DeckDrawRequestEvent>>()));
            Assert.That(deck.Response, Is.SameAs(container.Resolve<EventPort<DeckDrawResponseEvent>>()));
        }

        [Test]
        public void MultiplePublicConstructorsUseTheVContainerSelection()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<MultipleConstructors>();
            using IObjectResolver container = builder.Build();
            MultipleConstructors hub = container.Resolve<MultipleConstructors>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Left, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(3));
        }

        [Test]
        public void InjectConstructorTakesPriorityOverLongerUnusedConstructor()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<PreferredConstructor>();
            using IObjectResolver container = builder.Build();
            PreferredConstructor hub = container.Resolve<PreferredConstructor>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(builder.Exists(typeof(EventPort<RightEvent>)), Is.False);
            Assert.That(builder.Count, Is.EqualTo(3));
        }

        [Test]
        public void InheritedInjectMethodRegistersChildHubRecursively()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<InheritedMethodHub>();
            using IObjectResolver container = builder.Build();
            InheritedMethodHub hub = container.Resolve<InheritedMethodHub>();
            Assert.That(hub.Left, Is.SameAs(container.Resolve<LeftHub>()));
            Assert.That(hub.Right, Is.SameAs(container.Resolve<EventPort<RightEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(5));
        }

        [Test]
        public void ConstructorAndAllInjectMethodsContributeDependencies()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<MixedInjectionHub>();
            using IObjectResolver container = builder.Build();
            MixedInjectionHub hub = container.Resolve<MixedInjectionHub>();
            Assert.That(hub.Shared, Is.SameAs(hub.Left.Shared));
            Assert.That(hub.Right, Is.SameAs(container.Resolve<EventPort<RightEvent>>()));
            Assert.That(hub.LastInjected, Is.SameAs(hub.Shared));
            Assert.That(builder.Count, Is.EqualTo(5));
        }

        [Test]
        public void InjectMethodAllowsNonPublicConstructor()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<PrivateConstructorMethodHub>();
            using IObjectResolver container = builder.Build();
            Assert.That(container.Resolve<PrivateConstructorMethodHub>().Shared,
                Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
        }

        [Test]
        public void ZeroArgumentNonPublicConstructorIsRejected() => AssertInvalidHub<NoPublicConstructor>();

        [Test]
        public void MultipleInjectConstructorsAreRejected() => AssertInvalidHub<MultipleInjectConstructors>();

        [Test]
        public void ConstructorAndSingleInjectArgumentRegisterBothPorts()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<SingleParameterMembers>();
            using IObjectResolver container = builder.Build();
            SingleParameterMembers hub = container.Resolve<SingleParameterMembers>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Left, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(3));

            XDocument document = EventHubLinkerProcessor.CreateDocument(new[] { typeof(SingleParameterMembers) });
            Assert.That(document.Descendants("type").Select(element => (string)element.Attribute("fullname")),
                Does.Contain(typeof(SingleParameterMembers).FullName.Replace('+', '/')));
        }

        [Test]
        public void ConstructorAndInheritedPrivateInjectArgumentAreCombined()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<InheritedConstructorHub>();
            using IObjectResolver container = builder.Build();
            InheritedConstructorHub hub = container.Resolve<InheritedConstructorHub>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Left, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(3));

            XDocument document = EventHubLinkerProcessor.CreateDocument(new[] { typeof(InheritedConstructorHub) });
            Assert.That(document.Descendants("type").Select(element => (string)element.Attribute("fullname")),
                Does.Contain(typeof(SingleInjectionBase).FullName.Replace('+', '/')));
        }

        [Test]
        public void SharedPortAcrossConstructorAndInjectMethodCountsBothArguments()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<RepeatedConstructorInjectionHub>();
            using IObjectResolver container = builder.Build();
            RepeatedConstructorInjectionHub hub = container.Resolve<RepeatedConstructorInjectionHub>();
            Assert.That(hub.First, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Second, Is.SameAs(hub.First));
            Assert.That(builder.Count, Is.EqualTo(2));
        }

        [Test]
        public void UnselectedConstructorDoesNotSatisfyTheHubCondition()
        {
            AssertInvalidHub<UnusedConstructorHub>();
            Assert.Throws<InvalidOperationException>(() =>
                EventHubLinkerProcessor.CreateDocument(new[] { typeof(UnusedConstructorHub) }));
        }

        [Test]
        public void SelectedNonPublicConstructorArgumentsSatisfyTheHubCondition()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<PrivateConstructorHub>();
            using IObjectResolver container = builder.Build();
            PrivateConstructorHub hub = container.Resolve<PrivateConstructorHub>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Left, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(3));
        }

        [Test]
        public void SeparateSingleArgumentInjectMethodsRegisterBothPorts()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<SplitInjectionHub>();
            using IObjectResolver container = builder.Build();
            SplitInjectionHub hub = container.Resolve<SplitInjectionHub>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Left, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(3));

            XDocument document = EventHubLinkerProcessor.CreateDocument(new[] { typeof(SplitInjectionHub) });
            Assert.That(document.Descendants("type").Select(element => (string)element.Attribute("fullname")),
                Does.Contain(typeof(SplitInjectionHub).FullName.Replace('+', '/')));
        }

        [Test]
        public void InheritedInjectMethodArgumentsAreIncludedInTheTotal()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<InheritedSplitInjectionHub>();
            using IObjectResolver container = builder.Build();
            InheritedSplitInjectionHub hub = container.Resolve<InheritedSplitInjectionHub>();
            Assert.That(hub.Shared, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Left, Is.SameAs(container.Resolve<EventPort<LeftEvent>>()));
            Assert.That(builder.Count, Is.EqualTo(3));
        }

        [Test]
        public void SharedPortAcrossInjectMethodsIsRegisteredOnce()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<RepeatedInjectionHub>();
            using IObjectResolver container = builder.Build();
            RepeatedInjectionHub hub = container.Resolve<RepeatedInjectionHub>();
            Assert.That(hub.First, Is.SameAs(container.Resolve<EventPort<SharedEvent>>()));
            Assert.That(hub.Second, Is.SameAs(hub.First));
            Assert.That(builder.Count, Is.EqualTo(2));
        }

        [Test]
        public void SingleInjectMethodArgumentIsRejected() => AssertInvalidHub<SingleInjectionHub>();

        [Test]
        public void StaticInjectMethodDoesNotSatisfyTheHubCondition() => AssertInvalidHub<StaticInjectHub>();

        [Test]
        public void UnsupportedInjectMethodDependencyIsRejected() => AssertInvalidHub<UnsupportedInjectDependency>();

        [Test]
        public void OneParameterIsRejected() => AssertInvalidHub<OneParameter>();

        [Test]
        public void ZeroParametersAreRejected() => AssertInvalidHub<ZeroParameters>();

        [Test]
        public void MissingAttributeIsRejected() => AssertInvalidHub<UnmarkedHub>();

        [Test]
        public void UnsupportedDependencyIsRejected() => AssertInvalidHub<UnsupportedDependency>();

        [Test]
        public void InvalidHubLifetimeIsRejected() => AssertInvalidHub<InvalidLifetimeHub>();

        [Test]
        public void InvalidIndividualLifetimeIsRejected()
        {
            ContainerBuilder builder = new();
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.RegisterEvent<SharedEvent>((Lifetime)999));
            Assert.That(builder.Count, Is.Zero);
        }

        [Test]
        public void CircularDependencyReportsBothHubs()
        {
            ContainerBuilder builder = new();
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => builder.RegisterEventHub<CycleA>());
            Assert.That(exception.Message, Does.Contain(nameof(CycleA)).And.Contain(nameof(CycleB)));
            Assert.That(builder.Count, Is.Zero);
        }

        [Test]
        public void LinkXmlContainsNestedHubsAndGenericDefinitionsWithoutDuplicates()
        {
            var document = EventHubLinkerProcessor.CreateDocument(new[]
            {
                typeof(RootHub), typeof(LeftHub), typeof(GenericHub<SharedEvent, LeftEvent>),
                typeof(GenericHub<SharedEvent, RightEvent>)
            });
            string[] preservedNames = document.Descendants("type").Select(element => (string)element.Attribute("fullname")).ToArray();
            Assert.That(preservedNames, Does.Contain(typeof(RightHub).FullName.Replace('+', '/')));
            Assert.That(preservedNames, Does.Contain(typeof(GenericHub<,>).FullName.Replace('+', '/')));
            Assert.That(preservedNames, Does.Contain(typeof(EventPort<>).FullName));
            Assert.That(preservedNames.Distinct().Count(), Is.EqualTo(preservedNames.Length));
            Assert.That(EventHubLinkerProcessor.CreateDocument(new[] { typeof(RootHub) }).ToString(),
                Is.EqualTo(EventHubLinkerProcessor.CreateDocument(new[] { typeof(LeftHub), typeof(RootHub) }).ToString()));
        }

        [Test]
        public void LinkXmlValidatesConstructorRules()
        {
            Assert.Throws<InvalidOperationException>(() => EventHubLinkerProcessor.CreateDocument(new[] { typeof(OneParameter) }));
        }

        [Test]
        public void LinkXmlPreservesPrivateInjectMethodsInBaseTypesAndTheirDependencies()
        {
            XDocument document = EventHubLinkerProcessor.CreateDocument(new[]
            {
                typeof(InheritedMethodHub), typeof(DeckDrawEventHub)
            });
            string[] preservedNames = document.Descendants("type").Select(element => (string)element.Attribute("fullname")).ToArray();
            Assert.That(preservedNames, Does.Contain(typeof(InjectionBase).FullName.Replace('+', '/')));
            Assert.That(preservedNames, Does.Contain(typeof(LeftHub).FullName.Replace('+', '/')));
            Assert.That(preservedNames, Does.Contain(typeof(EventExchanger<,>).FullName));
            Assert.That(document.Descendants("type").All(element => (string)element.Attribute("preserve") == "all"), Is.True);
        }

        [Test]
        public void LinkerCallbackIncludesPlayerHubsAndExcludesEditorFixtures()
        {
            string generatedPath = new EventHubLinkerProcessor().GenerateAdditionalLinkXmlFile(
                null, new UnityLinkerBuildPipelineData(EditorUserBuildSettings.activeBuildTarget, string.Empty));
            string[] preservedNames = XDocument.Load(generatedPath).Descendants("type")
                .Select(element => (string)element.Attribute("fullname")).ToArray();
            Assert.That(preservedNames, Does.Contain(typeof(InteractionEventHub).FullName));
            Assert.That(preservedNames, Does.Contain(typeof(EventPort<>).FullName));
            Assert.That(preservedNames, Does.Contain(typeof(EventExchanger<,>).FullName));
            Assert.That(preservedNames, Does.Not.Contain(typeof(RootHub).FullName.Replace('+', '/')));
        }

        static void AssertInvalidHub<THub>()
        {
            ContainerBuilder builder = new();
            Assert.Throws<InvalidOperationException>(() => builder.RegisterEventHub<THub>());
            Assert.That(builder.Count, Is.Zero);
        }

        public record SharedEvent(int Value) : EventObject;
        public record LeftEvent : EventObject;
        public record RightEvent : EventObject;
        public record ExchangeRequest(int Value) : RequestEvent;
        public record ExchangeResponse(int Value) : ResponseEvent;

        [EventHub(Lifetime.Singleton)]
        public record LeftHub(EventPort<SharedEvent> Shared, EventPort<LeftEvent> Left);
        [EventHub(Lifetime.Singleton)]
        public record RightHub(EventPort<SharedEvent> Shared, EventPort<RightEvent> Right);
        [EventHub(Lifetime.Singleton)]
        public record RootHub(LeftHub Left, RightHub Right);
        [EventHub(Lifetime.Scoped)]
        public record ScopedHub(EventPort<SharedEvent> Shared, EventPort<LeftEvent> Left);
        [EventHub(Lifetime.Singleton)]
        public record ConflictingRoot(LeftHub Left, ScopedHub Scoped);
        [EventHub(Lifetime.Transient)]
        public record TransientRoot(LeftHub Child, EventPort<RightEvent> Direct);
        [EventHub(Lifetime.Singleton)]
        public record GenericHub<TFirst, TSecond>(EventPort<TFirst> First, EventPort<TSecond> Second)
            where TFirst : EventObject where TSecond : EventObject;

        [EventHub(Lifetime.Singleton)]
        public class NoPublicConstructor { NoPublicConstructor() { } }
        [EventHub(Lifetime.Singleton)]
        public class MultipleConstructors
        {
            public EventPort<SharedEvent> Shared { get; }
            public EventPort<LeftEvent> Left { get; }
            public MultipleConstructors() { }
            public MultipleConstructors(EventPort<SharedEvent> shared, EventPort<LeftEvent> left)
            {
                Shared = shared;
                Left = left;
            }
        }

        [EventHub]
        public class PreferredConstructor
        {
            public EventPort<SharedEvent> Shared { get; }
            [Inject]
            public PreferredConstructor(EventPort<SharedEvent> shared, EventPort<LeftEvent> left) => Shared = shared;
            public PreferredConstructor(EventPort<SharedEvent> shared, EventPort<RightEvent> right, string unused) { }
        }

        public abstract class InjectionBase
        {
            public LeftHub Left { get; private set; }
            public EventPort<RightEvent> Right { get; private set; }
            [Inject]
            void Construct(LeftHub left, EventPort<RightEvent> right)
            {
                Left = left;
                Right = right;
            }
        }

        [EventHub]
        public class InheritedMethodHub : InjectionBase { }

        [EventHub]
        public class MixedInjectionHub
        {
            public EventPort<SharedEvent> Shared { get; }
            public LeftHub Left { get; private set; }
            public EventPort<RightEvent> Right { get; private set; }
            public EventPort<SharedEvent> LastInjected { get; private set; }
            public MixedInjectionHub(EventPort<SharedEvent> shared) => Shared = shared;
            [Inject]
            void Construct(LeftHub left, EventPort<RightEvent> right)
            {
                Left = left;
                Right = right;
            }
            [Inject]
            void Complete(EventPort<SharedEvent> shared) => LastInjected = shared;
        }

        [EventHub]
        public class PrivateConstructorMethodHub
        {
            public EventPort<SharedEvent> Shared { get; private set; }
            PrivateConstructorMethodHub() { }
            [Inject]
            void Construct(EventPort<SharedEvent> shared, EventPort<LeftEvent> left) => Shared = shared;
        }

        [EventHub]
        public class MultipleInjectConstructors
        {
            [Inject]
            public MultipleInjectConstructors() { }
            [Inject]
            public MultipleInjectConstructors(EventPort<SharedEvent> shared, EventPort<LeftEvent> left) { }
        }

        [EventHub]
        public class SingleParameterMembers
        {
            public EventPort<SharedEvent> Shared { get; }
            public EventPort<LeftEvent> Left { get; private set; }
            public SingleParameterMembers(EventPort<SharedEvent> shared) => Shared = shared;
            [Inject]
            void Construct(EventPort<LeftEvent> left) => Left = left;
        }

        [EventHub]
        public class InheritedConstructorHub : SingleInjectionBase
        {
            public EventPort<LeftEvent> Left { get; }
            [Inject]
            public InheritedConstructorHub(EventPort<LeftEvent> left) => Left = left;
            public InheritedConstructorHub(EventPort<LeftEvent> left, string unused) { }
        }

        [EventHub]
        public class RepeatedConstructorInjectionHub
        {
            public EventPort<SharedEvent> First { get; }
            public EventPort<SharedEvent> Second { get; private set; }
            public RepeatedConstructorInjectionHub(EventPort<SharedEvent> shared) => First = shared;
            [Inject]
            void Construct(EventPort<SharedEvent> shared) => Second = shared;
        }

        [EventHub]
        public class UnusedConstructorHub
        {
            [Inject]
            public UnusedConstructorHub(EventPort<SharedEvent> shared) { }
            public UnusedConstructorHub(EventPort<SharedEvent> shared, EventPort<LeftEvent> left) { }
        }

        [EventHub]
        public class PrivateConstructorHub
        {
            public EventPort<SharedEvent> Shared { get; }
            public EventPort<LeftEvent> Left { get; }
            PrivateConstructorHub(EventPort<SharedEvent> shared, EventPort<LeftEvent> left)
            {
                Shared = shared;
                Left = left;
            }
        }

        [EventHub]
        public class SplitInjectionHub
        {
            public EventPort<SharedEvent> Shared { get; private set; }
            public EventPort<LeftEvent> Left { get; private set; }
            [Inject]
            void SetShared(EventPort<SharedEvent> shared) => Shared = shared;
            [Inject]
            void SetLeft(EventPort<LeftEvent> left) => Left = left;
        }

        public abstract class SingleInjectionBase
        {
            public EventPort<SharedEvent> Shared { get; private set; }
            [Inject]
            void SetShared(EventPort<SharedEvent> shared) => Shared = shared;
        }

        [EventHub]
        public class InheritedSplitInjectionHub : SingleInjectionBase
        {
            public EventPort<LeftEvent> Left { get; private set; }
            [Inject]
            void SetLeft(EventPort<LeftEvent> left) => Left = left;
        }

        [EventHub]
        public class RepeatedInjectionHub
        {
            public EventPort<SharedEvent> First { get; private set; }
            public EventPort<SharedEvent> Second { get; private set; }
            [Inject]
            void SetFirst(EventPort<SharedEvent> shared) => First = shared;
            [Inject]
            void SetSecond(EventPort<SharedEvent> shared) => Second = shared;
        }

        [EventHub]
        public class SingleInjectionHub
        {
            [Inject]
            void Construct(EventPort<SharedEvent> shared) { }
        }

        [EventHub]
        public class StaticInjectHub
        {
            [Inject]
            static void Construct(EventPort<SharedEvent> shared, EventPort<LeftEvent> left) { }
        }

        [EventHub]
        public class UnsupportedInjectDependency
        {
            [Inject]
            void Construct(EventPort<SharedEvent> shared, string unsupported) { }
        }
        [EventHub(Lifetime.Singleton)]
        public record OneParameter(EventPort<SharedEvent> First);
        [EventHub(Lifetime.Singleton)]
        public record ZeroParameters;
        public record UnmarkedHub(EventPort<SharedEvent> First, EventPort<LeftEvent> Second);
        [EventHub(Lifetime.Singleton)]
        public record UnsupportedDependency(EventPort<SharedEvent> First, string Unsupported);
        [EventHub((Lifetime)999)]
        public record InvalidLifetimeHub(EventPort<SharedEvent> First, EventPort<LeftEvent> Second);
        [EventHub(Lifetime.Singleton)]
        public record CycleA(CycleB Next, EventPort<SharedEvent> Port);
        [EventHub(Lifetime.Singleton)]
        public record CycleB(CycleA Next, EventPort<LeftEvent> Port);
    }
}
