using AetherAlmachina.Skill;
using DIVFactor.Event;

namespace DConfig.StageLife.Event
{

    public record SkillActivateEvent(SkillData Data) : EventObject;
}