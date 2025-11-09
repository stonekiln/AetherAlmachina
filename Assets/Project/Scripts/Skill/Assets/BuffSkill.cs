using UnityEngine;

[CreateAssetMenu(fileName = "BuffSkill", menuName = "Skills/Buff Skill")]
public class BuffSkill : ScriptableObject
{
    public string skillName;
    public int cost;
    public float power;
    public Sprite icon;
}