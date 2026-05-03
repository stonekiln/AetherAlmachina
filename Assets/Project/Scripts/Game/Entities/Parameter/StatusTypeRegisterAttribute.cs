using System;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// Statusの各種フィールドが何のステータスを表すのか、紐づけを行うための属性
    /// </summary>
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