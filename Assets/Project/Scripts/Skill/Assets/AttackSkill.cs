using UnityEngine;

[CreateAssetMenu(fileName = "AttackSkill", menuName = "Skills/Attack Skill")]
public class AttackSkill : ScriptableObject
{
    public string skillName;
    public int cost;
    public float power;
    public Sprite icon;
}