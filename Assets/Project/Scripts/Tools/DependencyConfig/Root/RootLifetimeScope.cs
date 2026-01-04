using DivFacter.EntryPoint;
using DivFacter.Event;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<EventBus<PreStartEvent>>(Lifetime.Scoped);
        builder.RegisterEntryPoint<PreStartable>(Lifetime.Scoped);
        builder.Register<EventBus<InitializeEvent>>(Lifetime.Scoped);
        builder.RegisterEntryPoint<Initializable>(Lifetime.Scoped);
    }
}
