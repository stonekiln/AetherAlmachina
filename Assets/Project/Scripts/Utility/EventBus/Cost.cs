using R3;
using UnityEngine;

namespace EventBus.Cost
{
    public class AutoIncreaseEvent : ScriptableObject
    {
        public readonly Subject<int> Event = new();
    }
    public class BonusIncreaseEvent : ScriptableObject
    {
        public readonly Subject<int> Event = new();
    }
}