using System;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class StatusTypeRegisterAttribute : PropertyAttribute
    {
        public StatusType Type { get; init; }

        public StatusTypeRegisterAttribute(StatusType type)
        {
            Type = type;
        }
    }
}