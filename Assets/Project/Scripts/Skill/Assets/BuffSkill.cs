using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "Skills/Buff Skill")]
public class BuffSkill : SkillData
{
    public override void Activate()
    {
        Debug.Log(skillName + "が発動しました");
    }
}