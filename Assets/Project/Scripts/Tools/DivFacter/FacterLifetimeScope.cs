using VContainer;
using VContainer.Unity;

namespace DivFacter
{
    public abstract class FactorLifetimeScope : LifetimeScope
    {
        protected abstract void Install(IContainerBuilder builder);
        protected abstract void Register(IContainerBuilder builder);
        protected abstract void Reserve(IContainerBuilder builder);

        protected override void Configure(IContainerBuilder builder)
        {
            Install(builder);
            Register(builder);
            Reserve(builder);
        }
    }
}
