using AetherAlmachina.Entities.Brain;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// エンティティのデータ
    /// </summary>
    [CreateAssetMenu(fileName = "Data", menuName = "Entity/Data")]
    public class EntityData : ScriptableObject
    {
        [field: SerializeField] public StatusAsset StatusAsset { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public EnemyBrain Brain { get; private set; }
    }
}