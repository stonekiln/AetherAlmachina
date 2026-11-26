using DConfig.StageLife.Event;
using DIVFactor.Lifecycle;
using R3;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// 味方側エンティティを配置するクラス
    /// </summary>
    public class FriendlyLayout : EntityLayout<FriendlyLayoutEvent>, ISubscriberComponent
    {
        public void Subscribe(SubscriptionContext context)
        {
            layoutEvent.Subscribe(req => Arrange(req.Friendly, transform.position, settings.Friendly.layoutSize, true)).AddTo(context.Lifetime);
        }
    }
}
