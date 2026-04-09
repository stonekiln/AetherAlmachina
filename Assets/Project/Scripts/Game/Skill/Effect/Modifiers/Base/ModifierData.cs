using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public interface IModifierTypeData
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public string DisplayUnit { get; }
        public StatusType StatusTypeKey { get; }
    }

    public interface IModifierData : IModifierTypeData
    {
        public float Value { get; }
        public Type ModifierType { get; }
    }

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

        public DispelModifier Enchant(ICombatInteraction user, ICombatInteraction target)
        {
            return type.ModifierType.Enchant(user, target, this);
        }
    }
}