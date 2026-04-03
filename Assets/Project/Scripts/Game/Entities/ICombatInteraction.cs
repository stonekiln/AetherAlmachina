using AetherAlmachina.Entities.Parameter;
using DConfig.EntityLife.Event;

namespace AetherAlmachina.Entities
{
    public interface ICombatInteraction
    {
        public int SiblingIndex { get; }
        public TargetingEventBundle Targeting { get; }
        public Status Status { get; }
        public void Attack(Entity target, float skillPower);
    }
}