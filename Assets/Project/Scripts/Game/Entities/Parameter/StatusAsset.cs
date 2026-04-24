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
        [SerializeField][StatusTypeRegister(StatusType.Power)] float power;

        void OnValidate()
        {
            //上記のフィールドの属性を読み取って取り扱いやすいデータ構造に変換しておく
            foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                StatusTypeRegisterAttribute attribute = field.GetCustomAttribute<StatusTypeRegisterAttribute>();
                if (attribute != null)
                {
                    BaseStatus[attribute.Type] = Convert.ToSingle(field.GetValue(this));
                }
            }
        }
    }
}