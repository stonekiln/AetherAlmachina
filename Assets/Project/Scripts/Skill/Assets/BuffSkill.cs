using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "Skills/Buff Skill")]
public class BuffSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(SkillName + "が発動しました");
    }
}