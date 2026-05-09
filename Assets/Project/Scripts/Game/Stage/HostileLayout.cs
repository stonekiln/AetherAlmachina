using System.Collections.Generic;
using DIVFactor.Event;
using DIVFactor.Injectable;
using DConfig.StageLife.Event;
using UnityEngine;
using R3;

namespace AetherAlmachina.Stage
{
    /// <summary>
    /// 敵側エンティティを配置するクラス
    /// </summary>
    public class HostileLayout : EntityLayout<HostileLayoutEvent>
    {
        public override void Injection(InjectableResolver resolver)
        {
            base.Injection(resolver);
            
            layoutEventBus.Subscribe(req => Arrange(req.Hostile, transform.position, settings.Hostile.layoutSize, false)).AddTo(this);
        }
    }
}
