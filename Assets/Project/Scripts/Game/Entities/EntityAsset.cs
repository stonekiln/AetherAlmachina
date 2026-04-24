using AetherAlmachina.Entities.Brain;
using AetherAlmachina.Entities.Parameter;
using UnityEngine;

namespace AetherAlmachina.Entities
{
    /// <summary>
    /// エンティティのデータを保持する
    /// </summary>
    [CreateAssetMenu(fileName = "Data", menuName = "Entity/Data")]
    public class EntityAsset : ScriptableObject
    {
        [field: SerializeField] public StatusAsset StatusAsset { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public EnemyBrain Brain { get; private set; }
    }
}