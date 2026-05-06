using System.Collections.Generic;
using AetherAlmachina.Skill.Effect;
using UnityEngine;

namespace AetherAlmachina.Skill
{
    /// <summary>
    /// スキルの情報を保持する
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/SkillData")]
    public class SkillAsset : ScriptableObject
    {
        /// <summary>
        /// スキル名
        /// </summary>
        [field: SerializeField] public string SkillName { get; private set; }
        /// <summary>
        /// スキルの説明文
        /// </summary>
        // デバッグ用にEffectQueueの内容を説明すること
        [field: SerializeField, TextArea(5, 5)] public string Description { get; private set; }
        /// <summary>
        /// スキルの発動コスト
        /// </summary>
        [field: SerializeField] public int Cost { get; private set; }
        /// <summary>
        /// スキルアイコン
        /// </summary>
        [field: SerializeField] public Sprite Icon { get; private set; }
        /// <summary>
        /// スキルの最初の効果対象
        /// </summary>
        // スキルの効果を発現させるために必ず最初に対象を取る必要がある
        [field: SerializeField] public LockOnParameter InitialLockOn { get; private set; }
        /// <summary>
        /// スキルの効果の内容。エフェクトの発動順。
        /// </summary>
        [field: SerializeField] public List<EffectData> EffectQueue { get; private set; }
    }
}