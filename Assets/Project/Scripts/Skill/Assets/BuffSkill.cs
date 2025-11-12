using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "Skills/Buff Skill")]
public class BuffSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(skillName + "が発動しました");
    }
}