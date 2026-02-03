using SKill;
using UnityEngine;

/// <summary>
/// 防御スキル
/// </summary>
[CreateAssetMenu(fileName = "GuardSkill", menuName = "Skills/Guard Skill")]
public class GuardSkill : SkillBase
{
    public override void Activate(Entity owner, Entity target)
    {
        Debug.Log(skillName + "が発動しました");
        Debug.Log(owner + "が" + target + "にシールドを付与しました");
    }
    public override TargetFlag TargetData() => TargetFlag.Friendly;
}