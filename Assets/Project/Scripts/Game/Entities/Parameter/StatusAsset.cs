using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AetherAlmachina.Deck;
using UnityEngine;
using Utility;

namespace AetherAlmachina.Entities.Parameter
{
    public abstract class StatusBase : ScriptableObject
    {
        [field: SerializeField] public DeckList Deck { get; private set; }
        public Dictionary<StatusType, float> BaseStatus { get; protected set; } = new();
    }

    /// <summary>
    /// エンティティのパラメータ
    /// </summary>
    [CreateAssetMenu(fileName = "Status", menuName = "Entity/Status")]
    public class StatusAsset : StatusBase
    {
        [field: SerializeField][StatusTypeRegister(StatusType.MaxHitPoint)] public int HitPoint { get; private set; }
        [field: SerializeField][StatusTypeRegister(StatusType.Attack)] public int Attack { get; private set; }
        [field: SerializeField][StatusTypeRegister(StatusType.Defence)] public int Defence { get; private set; }
        [StatusTypeRegister(StatusType.Power)] public float Power { get; private set; } = 1;

        void OnValidate()
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                StatusTypeRegisterAttribute attribute = prop.GetCustomAttribute<StatusTypeRegisterAttribute>();

                if (attribute != null)
                {
                    object value = prop.GetValue(this);
                    BaseStatus[attribute.Type] = Convert.ToSingle(value);
                }
            }
        }
    }
}