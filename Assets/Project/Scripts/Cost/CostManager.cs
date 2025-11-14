using System;
using UniRx;
using UnityEngine;

public class CostManager : MonoBehaviour
{
    [SerializeField] CostSettings costSettings;
    void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(costSettings.TimeSpan))
            .Subscribe(

            ).AddTo(this);
    }
}