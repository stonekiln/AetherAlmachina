using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

/// <summary>
/// 単体攻撃スキル
/// </summary>
[CreateAssetMenu(fileName = "SelfBuffSkill", menuName = "Skills/BuffSkill/")]
public class SelfBuffSkill : SkillBase, IBuffSkill
{
    [SerializeField] BuffSkillParam buffSkillParam;
    public BuffSkillParam BuffSkill => buffSkillParam;

    public override void Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile)
    {
        List<ICombatInteraction> targetList = new();
        hostile.Aggregate(0f, (max, entity) =>
        {
            switch (max.CompareTo(entity.Status.hitPoint))
            {
                case > 0:
                    return max;
                case 0:
                    targetList.Add(entity);
                    return entity.Status.hitPoint;
                case < 0:
                    return entity.Status.hitPoint;
            }
        });
        targetList[Random.Range(0, targetList.Count)].AttackEvent.Hit.Publish(new(BuffSkill.Activate));
    }
}