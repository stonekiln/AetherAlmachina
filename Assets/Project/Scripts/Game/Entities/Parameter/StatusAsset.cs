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
        [field: SerializeField][StatusTypeRegister(StatusType.MaxHitPoint)] int HitPoint { get; set; }
        [field: SerializeField][StatusTypeRegister(StatusType.Attack)] int Attack { get; set; }
        [field: SerializeField][StatusTypeRegister(StatusType.Defence)] int Defence { get; set; }
        [field: SerializeField][StatusTypeRegister(StatusType.Speed)] int Speed { get; set; }
        [StatusTypeRegister(StatusType.Power)] float Power { get; set; } = 1;

        void OnValidate()
        {
            //上記のフィールドの属性を読み取って取り扱いやすいデータ構造に変換しておく
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                StatusTypeRegisterAttribute attribute = prop.GetCustomAttribute<StatusTypeRegisterAttribute>();
                if (attribute != null)
                {
                    BaseStatus[attribute.Type] = Convert.ToSingle(prop.GetValue(this));
                }
            }
        }
    }
}