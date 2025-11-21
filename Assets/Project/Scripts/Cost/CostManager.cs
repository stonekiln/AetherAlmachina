using System;
using R3;
using UnityEngine;

public class CostManager : MonoBehaviour
{
    [SerializeField] CostSettings costSettings;
    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
            .Subscribe(_ => costSettings.Channel.OnNext(costSettings.Delta)).AddTo(this);
    }
}