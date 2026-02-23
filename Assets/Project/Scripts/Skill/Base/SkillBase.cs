using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [SerializeField] protected string skillName;
    public int cost;
    public Sprite icon;
    public abstract void Targeting(IEnumerable<ICombatInteraction> friendly, IEnumerable<ICombatInteraction> hostile);
}