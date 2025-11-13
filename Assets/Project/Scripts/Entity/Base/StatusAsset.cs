using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Status")]
public class StatusAsset : ScriptableObject
{
    [SerializeField] private float hitPoint;
    [SerializeField] private float attack;
    [SerializeField] private float defence;
    public float HitPoint => hitPoint;
    public float Attack => attack;
    public float Defence => defence;
}