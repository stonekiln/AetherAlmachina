using R3;

namespace EventBus
{
    public static class Cost
    {
        public static readonly Subject<int> autoIncrease=new();
        public static readonly Subject<int> bonusIncrease=new();
    }
}