using DIVFactor.Lifecycle;
using DConfig.StageLife.Event;
using R3;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// 味方側エンティティを配置するクラス
    /// </summary>
    public class FriendlyLayout : EntityLayout<FriendlyLayoutEvent>, ISubscriberComponent
    {
        public void Subscribe(UsageLifetime lifetime)
        {
            layoutEvent.Subscribe(req => Arrange(req.Friendly, transform.position, settings.Friendly.layoutSize, true)).AddTo(lifetime);
        }
    }
}
