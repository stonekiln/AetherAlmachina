using AetherAlmachina.Stage;
using DConfig.StageLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using R3;
using System;
using UnityEngine;

namespace AetherAlmachina.Cost
{
    /// <summary>
    /// 各エンティティにコストを設定するためのクラス
    /// </summary>
    public class CostManager : MonoBehaviour, ISubscriberComponent
    {
        EventPort<AutoIncreaseEvent> AutoIncrease;
        CostSettingsAsset costSettings;
        StageSettingsAsset settings;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out settings);
            resolver.Inject(out AutoIncrease);
            resolver.Injected.SubscribeLifecycle(Initialize, resolver.Fail);
        }

        public void Initialize() => costSettings = settings.CostSettings;

        public void Subscribe(SubscriptionContext context)
        {
            Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
                .Switch(AutoIncrease).Subscribe(_ => new(costSettings.Delta)).AddTo(context.Lifetime);
        }
    }
}