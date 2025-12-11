/// <summary>
/// ステータスのパラメータをコピーして変更可能にするためのクラス
/// </summary>
public class Status
{
    public float hitPoint;
    public float attack;
    public float defence;

    public Status(StatusAsset statusData)
    {
        hitPoint = statusData.HitPoint;
        attack = statusData.Attack;
        defence = statusData.Defence;
    }
}