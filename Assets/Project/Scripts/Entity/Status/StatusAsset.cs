using UnityEngine;

/// <summary>
/// エンティティのパラメータ
/// </summary>
[CreateAssetMenu(fileName = "Status", menuName = "Entity/Status")]
public class StatusAsset : ScriptableObject
{
    [SerializeField] private float hitPoint;
    [SerializeField] private float attack;
    [SerializeField] private float defence;
    [SerializeField] private DeckList deck;
    public float HitPoint => hitPoint;
    public float Attack => attack;
    public float Defence => defence;
    public DeckList Deck => deck;
}