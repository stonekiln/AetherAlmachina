using UnityEngine;

/// <summary>
/// 補助スキル
/// </summary>
[CreateAssetMenu(fileName = "BuffSkill", menuName = "Skills/Buff Skill")]
public class BuffSkill : SkillData
{
    public override void Activate()
    {
        Debug.Log(skillName + "が発動しました");
    }
}