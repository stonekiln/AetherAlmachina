using System;
using System.Collections.Generic;
using DIVFactor.Event;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// ステータスのパラメータをコピーして変更可能にするためのクラス
    /// </summary>
    public class StatusParameter
    {
        public float hitPoint;
        public int magicPoint;
        public readonly EventBus<MPFluctuationEvent> MPFluctuation;
        Dictionary<StatusType, float> BaseStatus { get; init; }
        public Dictionary<Type, ModifierParameter> Modifiers { get; init; }
        Dictionary<StatusType, float> ModifiedStatus => CalcStatus();

        public record MPFluctuationEvent : EventObject;

        public StatusParameter(StatusBase status)
        {
            MPFluctuation = new();
            BaseStatus = new(status.BaseStatus);
            Modifiers = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter(BaseStatus)},
                {typeof(RateModifierParameter),new RateModifierParameter(BaseStatus)}
            };
            hitPoint = ModifiedStatus[StatusType.MaxHitPoint];
            magicPoint = 0;
        }

        Dictionary<StatusType, float> CalcStatus()
        {
            Dictionary<StatusType, float> result = new();
            Dictionary<StatusType, float> flat = Modifiers[typeof(FlatModifierParameter)].Value;
            Dictionary<StatusType, float> rate = Modifiers[typeof(RateModifierParameter)].Value;

            foreach (StatusType type in BaseStatus.Keys)
            {
                result[type] = (BaseStatus[type] + flat[type]) * rate[type];
            }

            return result;
        }

        public float Get(StatusType type)
        {
            return ModifiedStatus[type];
        }

        public int GetInt(StatusType type)
        {
            return (int)ModifiedStatus[type];
        }
    }
}