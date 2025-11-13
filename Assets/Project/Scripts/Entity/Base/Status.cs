public class Status
{
    public float hitPoint;
    public float attack;
    public float defence;

    public Status(StatusData statusData)
    {
        hitPoint = statusData.HitPoint;
        attack = statusData.Attack;
        defence = statusData.Defence;
    }
}