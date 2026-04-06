using System;
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
        public Dictionary<Type, ModifierParameter> Modifiers { get; init; }
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
            Modifiers = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter(BaseStatus)},
                {typeof(PercentModifierParameter),new PercentModifierParameter(BaseStatus)}
            };
            hitPoint = MaxHitPoint;
            magicPoint = 0;
        }

        Dictionary<StatusType, float> CalcStatus()
        {
            Dictionary<StatusType, float> result = new();
            Dictionary<StatusType, float> flat = Modifiers[typeof(FlatModifierParameter)].Value;
            Dictionary<StatusType, float> percent = Modifiers[typeof(PercentModifierParameter)].Value;

            foreach (StatusType type in BaseStatus.Keys)
            {
                result[type] = (BaseStatus[type] + flat[type]) * percent[type];
            }

            return result;
        }
    }
}