using System;
using System.Collections.Generic;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    public class TriggerModifiers : ModifierStock<TriggerModifierData>
    {
        public TriggerModifiers()
        {
            Modifiers = new();
        }

        public override Action AddModifier(TriggerModifierData data)
        {
            Type modifierType = data.ModifierType;
            Type polarity = data.PolarityType;
            CreateKey(modifierType, polarity, data.TypeData);


            Modifiers[modifierType][polarity].Add(data.ModifyValue);

            Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
            return () => RemoveModifier(data);
        }
        protected override void RemoveModifier(TriggerModifierData data)
        {

            Modifiers[data.ModifierType][data.PolarityType].Remove(data.ModifyValue);

            Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + "の効果が解除された。");
        }
    }
}