using System;
using R3;
using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Entity/Status")]
public class StatusAsset : MonitoredEntity
{
    [SerializeField] private float hitPoint;
    [SerializeField] private float attack;
    [SerializeField] private float defence;
    [SerializeField] protected DeckList deck;
    public float HitPoint => hitPoint;
    public float Attack => attack;
    public float Defence => defence;
    public DeckList Deck => deck;
}

public class MonitoredEntity : ScriptableObject
{
    [NonSerialized] public ReactiveProperty<int> magicPoint = new();
    [NonSerialized] public readonly Subject<SkillData> SetOwnerEvent = new();
    [NonSerialized] public readonly Subject<float> SetHandPowerEvent = new();
}