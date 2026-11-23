using DIVFactor.Lifecycle;
using System;
using AetherAlmachina.Stage;
using DConfig.StageLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using R3;
using UnityEngine;

namespace AetherAlmachina.Cost
{
    /// <summary>
    /// 各エンティティにコストを設定するためのクラス
    /// </summary>
    public class CostManager : MonoBehaviour, IReactiveComponent
    {
        EventPort<AutoIncreaseEvent> AutoIncrease;
        CostSettingsAsset costSettings;
        StageSettingsAsset settings;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out settings);
            resolver.Inject(out AutoIncrease);

        }

        public void Initialize() => costSettings = settings.CostSettings;

        public void Subscribe(UsageLifetime lifetime)
        {
            Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
                .Switch(AutoIncrease).Subscribe(_ => new(costSettings.Delta)).AddTo(lifetime);
        }
    }
}