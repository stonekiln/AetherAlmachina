using UnityEngine;

/// <summary>
/// エンティティのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "Status", menuName = "Entity/Status")]
public class StatusAsset : ScriptableObject
{
    [field: SerializeField] public float HitPoint { get; private set; }
    [field: SerializeField] public float Attack { get; private set; }
    [field: SerializeField] public float Defence { get; private set; }
    [field: SerializeField] public DeckList Deck { get; private set; }
}