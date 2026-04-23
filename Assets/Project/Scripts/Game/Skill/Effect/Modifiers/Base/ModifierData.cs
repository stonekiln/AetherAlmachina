using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの情報を表すインターフェイス
    /// </summary>
    public interface IModifierTypeData
    {
        /// <summary>
        /// Modifierの名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 付与された際のModifierのアイコン
        /// </summary>
        public Sprite Icon { get; }
        /// <summary>
        /// Modifierの単位を表す
        /// </summary>
        public string DisplayUnit { get; }
        /// <summary>
        /// 変化するステータス
        /// </summary>
        public StatusType StatusTypeKey { get; }
    }

    public interface IModifierData : IModifierTypeData
    {
        /// <summary>
        /// 変化量
        /// </summary>
        public float Value { get; }
        /// <summary>
        /// Modifierの種類
        /// </summary>
        public Type ModifierType { get; }
    }

    /// <summary>
    /// Modifierの情報
    /// </summary>
    [Serializable]
    public class ModifierData : IModifierData
    {
        [SerializeField] ModifierAsset type;
        [field: SerializeField] public float Value { get; private set; }
        public Type ModifierType => type.ModifierType.GetType();
        public string Name => type.Name;
        public Sprite Icon => type.Icon;
        public string DisplayUnit => type.ModifierType.DisplayUnit;
        public StatusType StatusTypeKey => type.ModifierType.StatusTypeKey;

        /// <summary>
        /// Modifierを付与する対象を決める
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">付与対象候補</param>
        /// <returns>解除を行うための情報</returns>
        public DispelModifier Enchant(IEntityInteraction user, IEntityInteraction target)
        {
            return type.ModifierType.Enchant(user, target, this);
        }
    }
}