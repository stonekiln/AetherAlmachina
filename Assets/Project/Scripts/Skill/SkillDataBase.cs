using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

class SkillDataBase : MonoBehaviour
{
    AttackSkill[][] attackSkills;
    GuardSkill[][] guardSkills;
    HealSkill[][] healSkills;
    BuffSkill[][] buffSkills;

    void Awake()
    {
        attackSkills = Resources.LoadAll<AttackSkill>(Path.Combine("SkillCard", "SkillData", "Attack"))
                                .GroupBy(skill => skill.cost).Select(skill => skill.ToArray()).ToArray();
        guardSkills = Resources.LoadAll<GuardSkill>(Path.Combine("SkillCard", "SkillData", "Guard"))
                                .GroupBy(skill => skill.cost).Select(skill => skill.ToArray()).ToArray();
        healSkills = Resources.LoadAll<HealSkill>(Path.Combine("SkillCard", "SkillData", "Heal"))
                                .GroupBy(skill => skill.cost).Select(skill => skill.ToArray()).ToArray();
        buffSkills = Resources.LoadAll<BuffSkill>(Path.Combine("SkillCard", "SkillData", "Buff"))
                                .GroupBy(skill => skill.cost).Select(skill => skill.ToArray()).ToArray();
    }
    
    public AttackSkill Attack(int cost, int id)
    {
        return attackSkills[cost][id];
    }
    public GuardSkill Guard(int cost, int id)
    {
        return guardSkills[cost][id];
    }
    public HealSkill Heal(int cost, int id)
    {
        return healSkills[cost][id];
    }
    public BuffSkill buff(int cost,int id)
    {
        return buffSkills[cost][id];
    }
}