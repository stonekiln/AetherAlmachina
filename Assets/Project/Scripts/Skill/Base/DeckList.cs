using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// デッキを装備するためのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "DeckList", menuName = "Entity/DeckList")]
public class DeckList : ScriptableObject
{
    /// <summary>
    /// 同じスキルは数値で枚数を指定できる
    /// </summary>
    [Serializable]
    public class SkillSlot
    {
        [Range(1, 4)]
        public int number;
        [field: SerializeField] public SkillAsset Skill { get; private set; }
    }

    const int DeckLimit = 52;
    [SerializeField] List<SkillSlot> skills;

    private void OnValidate()
    {
        int skillNumber = skills.Aggregate(0, (previous, current) => previous + current.number);
        if (skillNumber > DeckLimit)
        {
            if (skills.Last().number == 1)
            {
                skills = skills.Take(skills.Count - 1).ToList();
            }
            else
            {
                skills.Last().number -= skillNumber - DeckLimit;
            }
            Debug.Log("カードの枚数制限に達しました");
        }
    }

    public IEnumerable<SkillData> ReadDeck(Entity owner)
    {
        return skills.Select(element => Enumerable.Range(0, element.number).Select(_ => new SkillData(element.Skill, owner))).SelectMany(skill => skill);
    }
}