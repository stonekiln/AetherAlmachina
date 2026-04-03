using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    [CreateAssetMenu(fileName = "NewBuffType", menuName = "Skill/BuffType")]
    public class ModifierType : ScriptableObject
    {
        [field: SerializeReference] public Modifier Modifier { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}