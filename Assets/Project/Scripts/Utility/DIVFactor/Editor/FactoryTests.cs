using System;
using System.Reflection;
using DIVFactor.Lifetime;
using DIVFactor.Pooling;
using DIVFactor.Spawner;
using NUnit.Framework;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    public class FactoryTests
    {
        [Test]
        public void TypedFactoryPassesThreeArgumentsAndResolvesTheResult()
        {
            IObjectResolver container = null;
            Type boundFactory = null;
            string selectedField = null;
            SpawnerInjector<DummyScope> injector = Injector((factoryType, fieldName) =>
            {
                boundFactory = factoryType;
                selectedField = fieldName;
                return arguments =>
                {
                    ContainerBuilder builder = new();
                    arguments.Register(builder);
                    builder.Register<Result>(VContainer.Lifetime.Singleton);
                    return container = builder.Build();
                };
            });
            injector.Inject(out Factory<string, int, double, Result> factory, "threeArguments");
            try
            {
                Result result = factory.Spawn("test", 42, 1.5);
                Assert.That((result.Text, result.Count, result.Scale), Is.EqualTo(("test", 42, 1.5)));
                Assert.That(boundFactory, Is.EqualTo(typeof(Factory<string, int, double, Result>)));
                Assert.That(selectedField, Is.EqualTo("threeArguments"));
            }
            finally { container?.Dispose(); }
        }

        [Test]
        public void VoidFactoryAcquiresExactlyOnceWithoutResolvingAResult()
        {
            int calls = 0;
            SpawnerInjector<DummyScope> injector = Injector((_, _) => arguments => { calls++; return null; });
            injector.Inject(out ActionFactory factory);
            factory.Spawn();
            Assert.That(calls, Is.EqualTo(1));
        }

        static SpawnerInjector<DummyScope> Injector(Func<Type, string, Func<LifetimeArguments, IObjectResolver>> bind)
            => (SpawnerInjector<DummyScope>)Activator.CreateInstance(typeof(SpawnerInjector<DummyScope>),
                BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { bind }, null);

        public class Result
        {
            public string Text { get; }
            public int Count { get; }
            public double Scale { get; }
            public Result(string text, int count, double scale) { Text = text; Count = count; Scale = scale; }
        }
        public class DummyScope : LifetimeObject
        {
            protected override void Install(ContainerInstaller installer) { }
            protected override void Register(ComponentRegister register) { }
        }
    }
}
