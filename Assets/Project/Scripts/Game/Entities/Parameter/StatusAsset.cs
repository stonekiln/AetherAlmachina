using AetherAlmachina.Deck;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// エンティティのパラメータ
    /// </summary>
    [CreateAssetMenu(fileName = "Status", menuName = "Entity/Status")]
    public class StatusAsset : ScriptableObject
    {
        [field: SerializeField] public int HitPoint { get; private set; }
        [field: SerializeField] public int Attack { get; private set; }
        [field: SerializeField] public int Defence { get; private set; }
        [field: SerializeField] public DeckList Deck { get; private set; }
    }
}