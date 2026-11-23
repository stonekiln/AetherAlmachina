using AetherAlmachina.Entities.Parameter;
using DConfig.EntityLife.Event;
using DIVFactor.Extensions;
using DIVFactor.Lifecycle;
using NUnit.Framework;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    public class ResourceUsageTests
    {
        [Test]
        public void ResourceUpdatesStopWithUsageAndDoNotReachTheNextInstance()
        {
            ContainerBuilder builder = new();
            builder.RegisterEventHub<ResourceUpdateEventHub>();
            using IObjectResolver container = builder.Build();
            ResourceUpdateEventHub events = container.Resolve<ResourceUpdateEventHub>();
            ResourceStatusParameter previous = new();
            using UsageLifetime first = new();
            previous.Subscribe(events, first);
            events.Cost.Request.OnNext(new(5));
            events.HP.Request.OnNext(new(10));
            Assert.That((previous.Cost, previous.HitPoint), Is.EqualTo((5, 10)));
            first.Dispose();
            ResourceStatusParameter current = new();
            using UsageLifetime second = new();
            current.Subscribe(events, second);
            events.Cost.Request.OnNext(new(2));
            events.HP.Request.OnNext(new(20));
            Assert.That((previous.Cost, previous.HitPoint), Is.EqualTo((5, 10)));
            Assert.That((current.Cost, current.HitPoint), Is.EqualTo((2, 20)));
        }
    }
}
