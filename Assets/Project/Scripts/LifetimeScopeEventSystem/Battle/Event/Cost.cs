namespace LSES.Battle.Event
{
    public record AutoIncreaseEvent(int Delta) : EventObject;
    public record BonusIncreaseEvent(int Delta) : EventObject;
}