using System.Collections.Generic;
using System.IO;
using UnityEngine;

class SkillDataBase : MonoBehaviour
{
    public List<SkillData>[,] Skills { get; private set; }
    const int attack = 0;
    const int Guard = 1;
    const int Heal = 2;
    const int Buff = 3;

    public void ReadDataBase()
    {
        Skills = new List<SkillData>[4, 14];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                Skills[i, j] = new List<SkillData>();
            }
        }
        foreach (AttackSkill skill in Resources.LoadAll<AttackSkill>(Path.Combine("SkillCard", "SkillData", "Attack")))
        {
            Skills[attack, skill.cost].Add(new SkillData(skill.Activate, skill.CreateObject()));
        }
        foreach (GuardSkill skill in Resources.LoadAll<GuardSkill>(Path.Combine("SkillCard", "SkillData", "Guard")))
        {
            Skills[Guard, skill.cost].Add(new SkillData(skill.Activate, skill.CreateObject()));
        }
        foreach (HealSkill skill in Resources.LoadAll<HealSkill>(Path.Combine("SkillCard", "SkillData", "Heal")))
        {
            Skills[Heal, skill.cost].Add(new SkillData(skill.Activate, skill.CreateObject()));
        }
        foreach (BuffSkill skill in Resources.LoadAll<BuffSkill>(Path.Combine("SkillCard", "SkillData", "Buff")))
        {
            Skills[Buff, skill.cost].Add(new SkillData(skill.Activate, skill.CreateObject()));
        }
    }
}