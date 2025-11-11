using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "Skills/Attack Skill")]
public class AttackSkill : SkillBase
{
    public override void Activate()
    {
        Debug.Log(name + "が発動しました");
    }
}