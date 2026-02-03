using SKill;
using UnityEngine;

/// <summary>
/// 補助スキル
/// </summary>
[CreateAssetMenu(fileName = "BuffSkill", menuName = "Skills/Buff Skill")]
public class BuffSkill : SkillBase
{
    public override void Activate(Entity owner, Entity target)
    {
        Debug.Log(skillName + "が発動しました");
    }
    public override TargetFlag TargetData() => TargetFlag.Friendly;
}