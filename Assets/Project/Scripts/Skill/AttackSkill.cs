using SKill;
using UnityEngine;

/// <summary>
/// 攻撃スキル
/// </summary>
[CreateAssetMenu(fileName = "AttackSkill", menuName = "Skills/Attack Skill")]
public class AttackSkill : SkillBase
{
    public override void Activate(Entity owner, Entity target)
    {
        Debug.Log(skillName + "が発動しました");
        owner.Attack(target, power);
    }

    public override TargetFlag TargetData() => TargetFlag.Hostile;
}