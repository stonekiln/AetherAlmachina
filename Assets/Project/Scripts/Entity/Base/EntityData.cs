using UnityEngine;

/// <summary>
/// エンティティのデータ
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Entity/Data")]
public class StatusData : ScriptableObject
{
    [SerializeField] StatusAsset statusAsset;
    [SerializeField] Sprite sprite;
    [SerializeField] EnemyBrain brain;
    public StatusAsset StatusAsset => statusAsset;
    public Sprite Sprite => sprite;
    public BrainBase Brain => brain;
}