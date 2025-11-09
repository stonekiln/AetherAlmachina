using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "Skills/Heal Skill")]
public class HealSkill : ScriptableObject
{
    public string skillName;
    public int cost;
    public float power;
    public Sprite icon;
}