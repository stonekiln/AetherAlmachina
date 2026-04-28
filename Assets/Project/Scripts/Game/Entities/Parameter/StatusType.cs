namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// エンティティのステータスの種類
    /// </summary>
    public enum StatusType
    {
        MaxHitPoint = 1,
        Attack = 2,
        Defence = 4,
        Speed = 8,
        CriticalRate = 16,
        CriticalDamage = 32,
        Power = 64,
        DamageTaken = 128,
        HealPower = 256,
        HealingReceived = 512,
    }
}