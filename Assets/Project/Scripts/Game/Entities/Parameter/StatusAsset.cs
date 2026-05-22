using System;
using System.Collections.Generic;
using System.Reflection;
using AetherAlmachina.Deck;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    public abstract class StatusBase : ScriptableObject
    {
        [field: SerializeField] public DeckListAsset Deck { get; private set; }
        public Dictionary<StatusType, float> BaseStatus { get; protected set; } = new();
    }

    /// <summary>
    /// エンティティのパラメータを保持する
    /// </summary>
    [CreateAssetMenu(fileName = "Status", menuName = "Entity/Status")]
    public class StatusAsset : StatusBase
    {
        [SerializeField][StatusTypeRegister(StatusType.MaxHitPoint)] int hitPoint;
        [SerializeField][StatusTypeRegister(StatusType.Attack)] int attack;
        [SerializeField][StatusTypeRegister(StatusType.Defence)] int defence;
        [SerializeField][StatusTypeRegister(StatusType.Speed)] int speed;
        [StatusTypeRegister(StatusType.CriticalRate)] float CriticalRate => 0.05f;
        [StatusTypeRegister(StatusType.CriticalDamage)] float CriticalDamage => 1.2f;
        [StatusTypeRegister(StatusType.Power)] float Power => 1f;
        [StatusTypeRegister(StatusType.DamageTaken)] float DamageTaken => 1f;
        [StatusTypeRegister(StatusType.HealPower)] float HealPower => 1f;
        [StatusTypeRegister(StatusType.HealingReceived)] float HealingReceived => 1f;

        void OnValidate()
        {
            //上記のフィールドとプロパティの属性を読み取って取り扱いやすいデータ構造に変換しておく
            foreach (MemberInfo member in GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                StatusTypeRegisterAttribute attribute = member.GetCustomAttribute<StatusTypeRegisterAttribute>();
                if (attribute != null)
                {
                    object value = member switch
                    {
                        FieldInfo field => field.GetValue(this),
                        PropertyInfo prop => prop.GetValue(this),
                        _ => null
                    };
                    BaseStatus[attribute.Type] = Convert.ToSingle(value);
                }
            }
        }
    }
}