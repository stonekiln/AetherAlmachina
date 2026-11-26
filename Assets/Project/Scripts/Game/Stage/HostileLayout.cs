using DConfig.StageLife.Event;
using DIVFactor.Lifecycle;
using R3;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// 敵側エンティティを配置するクラス
    /// </summary>
    public class HostileLayout : EntityLayout<HostileLayoutEvent>, ISubscriberComponent
    {
        public void Subscribe(SubscriptionContext context)
        {
            layoutEvent.Subscribe(req => Arrange(req.Hostile, transform.position, settings.Hostile.layoutSize, false)).AddTo(context.Lifetime);
        }
    }
}
