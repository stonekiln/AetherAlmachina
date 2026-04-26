using System;
using System.Collections.Generic;
using DIVFactor.Event;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// ステータスのパラメータをコピーして変更可能にするためのクラス
    /// </summary>
    public class StatusParameter
    {
        public float hitPoint;
        public int magicPoint;
        public EventBus<MPFluctuationEvent> MPFluctuation { get; private set; }
        Dictionary<StatusType, float> BaseStatus { get; init; }
        public Dictionary<Type, ModifierParameter> Modifiers { get; init; }
        /// <summary>
        /// コストが増加する際に呼び出されるイベントメッセージ
        /// </summary>
        public record MPFluctuationEvent : EventObject;

        public StatusParameter(StatusBase status)
        {
            MPFluctuation = new();
            BaseStatus = new(status.BaseStatus);
            Modifiers = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter()},
                {typeof(RateModifierParameter),new RateModifierParameter()}
            };
            hitPoint = Get(StatusType.MaxHitPoint);
            magicPoint = 0;
        }

        /// <summary>
        /// 指定した種類のステータスの数値を取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>取得した数値</returns>
        public float Get(StatusType type)
        {
            return (BaseStatus[type] + Modifiers[typeof(FlatModifierParameter)].GetValue(type)) * Modifiers[typeof(RateModifierParameter)].GetValue(type);
        }
        /// <summary>
        /// 指定した種類のステータスの数値を整数値で取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>取得した数値</returns>
        public int GetInt(StatusType type)
        {
            return (int)Get(type);
        }
    }
}