using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// Modifierの情報を保持する
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuffType", menuName = "Skill/BuffType")]
    public class ModifierAsset : ScriptableObject
    {
        /// <summary>
        /// Modifierの種類
        /// </summary>
        [field: SerializeReference] public ModifierBase Definition { get; private set; }
        /// <summary>
        /// バフ・デバフの定義
        /// </summary>
        [field: SerializeReference] public ModifierPolarity Polarity { get; private set; }
        /// <summary>
        /// Modifierの名称
        /// </summary>
        [field: SerializeField] public string Name { get; private set; }
        /// <summary>
        /// 付与された際のModifierのアイコン
        /// </summary>
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}