using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "Skills/Heal Skill")]
public class HealSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(SkillName + "が発動しました");
    }
}