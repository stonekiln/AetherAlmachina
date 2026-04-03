using System;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class EffectData
    {
        [field: SerializeReference] public SkillEffect Effect { get; private set; }
        [field: SerializeReference] public EffectParameter Parameter { get; private set; }

        public EffectData(SkillEffect effect, EffectParameter parameter)
        {
            Effect = effect;
            Parameter = parameter;
        }
    }
}