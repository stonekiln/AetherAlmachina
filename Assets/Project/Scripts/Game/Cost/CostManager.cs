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
    public class CostManager : MonoBehaviour, IInjectable
    {
        EventBus<AutoIncreaseEvent> AutoIncrease;
        CostSettingsAsset costSettings;

        public void Injection(InjectableResolver resolver)
        {
            costSettings = resolver.GetComponent<StageSettingsAsset>().CostSettings;
            resolver.Inject(out AutoIncrease);

            Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
                .Switch(AutoIncrease).Subscribe(_ => new(costSettings.Delta)).AddTo(this);
        }
    }
}