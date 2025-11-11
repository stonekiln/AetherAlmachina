using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "Skills/Heal Skill")]
public class HealSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(name + "が発動しました");
    }
}