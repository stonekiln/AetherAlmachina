using UnityEngine;

[CreateAssetMenu(fileName = "GuardSkill", menuName = "Skills/Guard Skill")]
public class GuardSkill : SkillData
{
    public override void Activate()
    {
        Debug.Log(skillName + "が発動しました");
    }
}