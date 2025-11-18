using System;
using R3;
using UnityEngine;

namespace EventBus.Card
{
    [CreateAssetMenu(fileName = "CardActive", menuName = "EventBus/CardActive")]
    public class CardActiveEvent : ScriptableObject
    {
        [NonSerialized] public readonly Subject<(Action action,int cost)> Add = new();
        [NonSerialized] public readonly Subject<bool> Event=new();
    }
}