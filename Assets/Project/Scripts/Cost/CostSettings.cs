using UnityEngine;

[CreateAssetMenu(fileName = "CostSettings", menuName = "GameSettings/CostSettings")]
public class CostSettings : ScriptableObject
{
    [SerializeField] int delta;
    [SerializeField] float timeSpan;
    public int Delta => delta;
    public float TimeSpan => timeSpan;
}