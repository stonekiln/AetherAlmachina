using System.Collections.Generic;
using AetherAlmachina.Skill.Effect;
using UnityEngine;

namespace AetherAlmachina.Skill
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
    public class SkillAsset : ScriptableObject
    {
        [field: SerializeField] public string SkillName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public LockOnParameter InitialTargeting { get; private set; }
        [field: SerializeField] public List<EffectData> EffectQue { get; private set; }
    }
}