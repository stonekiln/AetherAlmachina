using System;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class EffectData
    {
        public EffectData(SkillEffect effect, EffectParameter parameter)
        {
            Effect = effect;
            Parameter = parameter;
        }
        [field: SerializeReference] public SkillEffect Effect { get; private set; }
        [field: SerializeReference] public EffectParameter Parameter { get; private set; }
    }
}