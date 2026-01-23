using DIVFactor.Event;

/// <summary>
/// ステータスのパラメータをコピーして変更可能にするためのクラス
/// </summary>
public class Status
{
    public float hitPoint;
    public float attack;
    public float defence;
    public int magicPoint;
    public readonly EventBus<MPfluctuationEvent> MPfluctuation;
    public record MPfluctuationEvent : EventObject;


    public Status(StatusAsset statusData)
    {
        MPfluctuation = new();
        hitPoint = statusData.HitPoint;
        attack = statusData.Attack;
        defence = statusData.Defence;
        magicPoint = 0;
    }
}