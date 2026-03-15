using System;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect
{
    [Serializable]
    public class EffectData
    {
        [field: SerializeField] public SkillEffect Effect { get; private set; }
        [field: SerializeReference] public EffectParameter Parameter { get; private set; }
    }
}