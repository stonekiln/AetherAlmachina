using System.Collections.Generic;
using System.Linq;
using DConfig.EntityLife.Event;
using R3;
using UnityEngine;
using Utility;

/// <summary>
/// 単体攻撃スキル
/// </summary>
[CreateAssetMenu(fileName = "HPRefAttackSkill", menuName = "Skills/AttackSkill/StatusRef")]
public class HPRefAttackSkill : SkillBase, IAttackSkill
{
    [SerializeField] AttackSkillParam attackSkillParam;
    public AttackSkillParam AttackSkill => attackSkillParam;

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
                    targetList = new() { entity };
                    return entity.Status.hitPoint;
            }
        });
        AttackEventBundle test = targetList[Random.Range(0, targetList.Count - 1)].AttackEvent;
        test.Hit.Subscribe(_ => Debug.Log(true));
        test.Hit.Publish(new(AttackSkill.Activate));
    }
}