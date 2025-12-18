using VContainer;
using VContainer.Unity;

namespace DConfig.EnemyLife
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<Enemy>().UnderTransform(transform.parent);
            builder.RegisterComponentInHierarchy<HandVisualizer>().UnderTransform(transform.parent);
        }
    }
}
