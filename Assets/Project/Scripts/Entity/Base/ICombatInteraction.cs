using DConfig.EntityLife.Event;

public interface ICombatInteraction
{
    public int SiblingIndex { get; }
    public AttackEventBundle AttackEvent { get; }
    public Status Status { get; }
    public void Attack(Entity target, float skillPower);
    public void Hit(float attackerAttack, float power);
}