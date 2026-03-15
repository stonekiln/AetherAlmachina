using AetherAlmachina.Entities.Status;
using DConfig.EntityLife.Event;

namespace AetherAlmachina.Entities
{
    public interface ICombatInteraction
    {
        public int SiblingIndex { get; }
        public TargetingEventBundle Targeting { get; }
        public StatusParameter Status { get; }
        public void Attack(Entity target, float skillPower);
        public void Hit(float attackerAttack, float power);
    }
}