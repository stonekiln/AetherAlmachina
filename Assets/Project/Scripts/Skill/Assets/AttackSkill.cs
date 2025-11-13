using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "Skills/Attack Skill")]
public class AttackSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(SkillName + "が発動しました");
        owner.Attack(Power);
    }
}