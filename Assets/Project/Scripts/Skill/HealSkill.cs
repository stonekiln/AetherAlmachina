using SKill;
using UnityEngine;

/// <summary>
/// 回復スキル
/// </summary>
[CreateAssetMenu(fileName = "HealSkill", menuName = "Skills/Heal Skill")]
public class HealSkill : SkillBase
{
    public override void Activate(Entity owner, Entity target)
    {
        Debug.Log(skillName + "が発動しました");
    }
    public override TargetFlag TargetData() => TargetFlag.Friendly;
}