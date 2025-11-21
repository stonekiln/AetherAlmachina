using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "Skills/Attack Skill")]
public class AttackSkill : SkillData
{
    public override void Activate()
    {
        Debug.Log(skillName + "が発動しました");
        owner.Attack(power);
    }
}