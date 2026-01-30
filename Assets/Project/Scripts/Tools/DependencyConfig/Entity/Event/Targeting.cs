using DIVFactor.Event;

namespace DConfig.EntityLife.Event
{
    public enum Target
    {
        Friendly,
        Hostile
    }
    public record TargetingEvent(Target Tag) : EventObject;
}