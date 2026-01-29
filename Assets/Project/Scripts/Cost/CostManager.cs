using System;
using DConfig.StageLife.Event;
using DIVFactor.Event;
using DIVFactor.Injectable;
using R3;
using UnityEngine;

/// <summary>
/// 各エンティティにコストを設定するためのクラス
/// </summary>
public class CostManager : MonoBehaviour, IInjectable
{
    EventBus<AutoIncreaseEvent> AutoIncrease;
    CostSettings costSettings;

    public void Injection(InjectableResolver resolver)
    {
        costSettings = resolver.GetComponent<StageSettings>().CostSettings;
        resolver.Inject(out AutoIncrease);

        Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
            .Subscribe(_ => AutoIncrease.OnNext(new(costSettings.Delta))).AddTo(this);
    }
}