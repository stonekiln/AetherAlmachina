namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// エンティティのステータスの種類
    /// </summary>
    public enum StatusType
    {
        MaxHitPoint = 1,
        Shield = 2,
        Attack = 4,
        Defence = 8,
        Speed = 16,
        CriticalRate = 32,
        CriticalDamage = 64,
        Power = 128,
        DamageTaken = 256,
        HealPower = 512,
        HealingReceived = 1024,
    }
}