using UnityEngine;

[CreateAssetMenu(fileName = "GuardSkill", menuName = "Skills/Guard Skill")]
public class GuardSkill : ScriptableObject
{
    public string skillName;
    public int cost;
    public float power;
    public Sprite icon;
}