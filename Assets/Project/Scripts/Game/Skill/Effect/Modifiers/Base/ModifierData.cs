using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill.Effect.Contracts;
using EditorExtends.Attribute;
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
        [SerializeField] ModifierAsset Type;
        [field: SerializeField, NonSliderRange(0, 100)] public float Value { get; private set; }
        public Type ModifierType => Type.ModifierType.GetType();
        public string Name => Type.Name;
        public Sprite Icon => Type.Icon;
        public string DisplayUnit => Type.ModifierType.DisplayUnit;
        public StatusType StatusTypeKey => Type.ModifierType.StatusTypeKey;

        public DispelModifier Enchant(ICombatInteraction user, ICombatInteraction target)
        {
            return Type.ModifierType.Enchant(user, target, this);
        }
    }
}