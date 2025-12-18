using DConfig.BattleLife.Installer;
using DivFacter.Injectable;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DConfig.BattleLife
{
    public class BattleLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            new CostEventInstaller().Install(builder);

            builder.RegisterComponentInHierarchy<CostManager>();
            builder.InjectMonoBehaviors(FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None));
        }
    }
}
