using System;
using R3;
using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Status")]
public class StatusAsset : MonitoredEntity
{
    [SerializeField] private float hitPoint;
    [SerializeField] private float attack;
    [SerializeField] private float defence;
    public float HitPoint => hitPoint;
    public float Attack => attack;
    public float Defence => defence;
}

public class MonitoredEntity : ScriptableObject
{
    [NonSerialized] public ReactiveProperty<int> magicPoint = new();
}