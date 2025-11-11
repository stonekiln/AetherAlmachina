using UnityEngine;

[CreateAssetMenu(fileName = "GuardSkill", menuName = "Skills/Guard Skill")]
public class GuardSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(name + "が発動しました");
    }
}