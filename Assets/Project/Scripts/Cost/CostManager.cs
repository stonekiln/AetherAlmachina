using System;
using DConfig.BattleLife.Event;
using DivFacter.Event;
using DivFacter.Injectable;
using R3;
using UnityEngine;

/// <summary>
/// 各エンティティにコストを設定するためのクラス
/// </summary>
public class CostManager : MonoBehaviour, IInjectable
{
    EventBus<AutoIncreaseEvent> AutoIncrease;
    [SerializeField] CostSettings costSettings;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out AutoIncrease);
    }

    void OnEnable()
    {
        Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
            .Subscribe(_ => AutoIncrease.Publish(new(costSettings.Delta))).AddTo(this);
    }
}