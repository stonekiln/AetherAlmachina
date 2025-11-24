using System;
using LSES;
using LSES.Battle.Event;
using R3;
using UnityEngine;
using VContainer;

public class CostManager : MonoBehaviour
{
    [Inject] EventBus<AutoIncreaseEvent> AutoIncrease;
    
    [SerializeField] CostSettings costSettings;

    void OnEnable()
    {
        Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
            .Subscribe(_ => AutoIncrease.Publish(new(costSettings.Delta))).AddTo(this);
    }
}