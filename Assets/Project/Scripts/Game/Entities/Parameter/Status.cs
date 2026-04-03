using System.Collections.Generic;
using DIVFactor.Event;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// ステータスのパラメータをコピーして変更可能にするためのクラス
    /// </summary>
    public class Status
    {
        public float hitPoint;
        public int magicPoint;
        public readonly EventBus<MPFluctuationEvent> MPFluctuation;
        Dictionary<StatusType, float> BaseStatus { get; init; }
        public FlatModifierParameter FlatModifier { get; init; }
        public PercentModifierParameter PercentModifier { get; init; }
        public int MaxHitPoint => (int)ModifiedStatus[StatusType.MaxHitPoint];
        public int Attack => (int)ModifiedStatus[StatusType.Attack];
        public int Defence => (int)ModifiedStatus[StatusType.Defence];
        public float Power => ModifiedStatus[StatusType.Power];
        Dictionary<StatusType, float> ModifiedStatus => CalcStatus();

        public record MPFluctuationEvent : EventObject;

        public Status(StatusAsset statusAsset)
        {
            MPFluctuation = new();
            BaseStatus = new(){
                {StatusType.MaxHitPoint,statusAsset.HitPoint},
                {StatusType.Attack,statusAsset.Attack},
                {StatusType.Defence,statusAsset.Defence},
                {StatusType.Power,1}
            };
            FlatModifier = new(BaseStatus);
            PercentModifier = new(BaseStatus);
            hitPoint = MaxHitPoint;
            magicPoint = 0;
        }

        Dictionary<StatusType, float> CalcStatus()
        {
            Dictionary<StatusType, float> result = new();
            Dictionary<StatusType, float> flat = FlatModifier.Value;
            Dictionary<StatusType, float> percent = PercentModifier.Value;

            foreach (StatusType type in BaseStatus.Keys)
            {
                result[type] = (BaseStatus[type] + flat[type]) * percent[type];
            }

            return result;
        }
    }
}