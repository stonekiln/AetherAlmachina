using System;
using EventBus;
using R3;
using UnityEngine;

public class CostManager : MonoBehaviour
{
    [SerializeField] CostSettings costSettings;
    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
            .Subscribe(_ => Cost.autoIncrease.OnNext(costSettings.CostDelta)).AddTo(this);
    }
}