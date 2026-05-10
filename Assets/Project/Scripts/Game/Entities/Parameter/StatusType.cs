namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// エンティティのステータスの種類
    /// </summary>
    public enum StatusType
    {
        MaxHitPoint = 1,
        Shield = 2,
        Disable = 4,
        Attack = 8,
        Defence = 16,
        Speed = 32,
        CriticalRate = 64,
        CriticalDamage = 128,
        Power = 256,
        DamageTaken = 512,
        HealPower = 1024,
        HealingReceived = 2028,
    }
}