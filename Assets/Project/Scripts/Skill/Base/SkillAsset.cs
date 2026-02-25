using System.Collections.Generic;
using Skill.Effects;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skills/Skill")]
public class SkillAsset : ScriptableObject
{
    [field: SerializeField] public string SkillName { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Targeting InitialTargeting { get; private set; }
    [field: SerializeField] public List<SkillEffect> EffectQue { get; private set; }
}