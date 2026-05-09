using System.Collections.Generic;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DConfig.StageLife.Event;
using UnityEngine;
using R3;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// 味方側エンティティを配置するクラス
    /// </summary>
    public class FriendlyLayout : EntityLayout<FriendlyLayoutEvent>
    {
        public override void Injection(InjectableResolver resolver)
        {
            base.Injection(resolver);

            layoutEventBus.Subscribe(req => Arrange(req.Friendly, transform.position, settings.Friendly.layoutSize, true)).AddTo(this);
        }
    }
}
