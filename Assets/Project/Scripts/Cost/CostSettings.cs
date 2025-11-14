using UnityEngine;

[CreateAssetMenu(fileName = "CostSettings", menuName = "GameSettings/CostSettings")]
public class CostSettings : ScriptableObject
{
    [SerializeField] int costDelta;
    [SerializeField] float timeSpan;
    public int CostDelta => costDelta;
    public float TimeSpan => timeSpan;
}