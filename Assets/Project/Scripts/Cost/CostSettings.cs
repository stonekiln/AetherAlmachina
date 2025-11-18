using EventBus.Cost;
using UnityEngine;

[CreateAssetMenu(fileName = "CostSettings", menuName = "GameSettings/CostSettings")]
public class CostSettings : AutoIncreaseEvent
{
    [SerializeField] int delta;
    [SerializeField] float timeSpan;
    public int Delta => delta;
    public float TimeSpan => timeSpan;
}