using System;
using R3;
using UnityEngine;

namespace EventBus.Card
{
    [CreateAssetMenu(fileName = "CardSelectEvent", menuName = "EventBus/CardSelect")]
    public class CardSelectEvent : ScriptableObject
    {
        [NonSerialized] public readonly Subject<int> Add=new();
        [NonSerialized] public readonly Subject<int> Remove=new();
        [NonSerialized] public readonly Subject<bool> Invoke = new();
    }
}