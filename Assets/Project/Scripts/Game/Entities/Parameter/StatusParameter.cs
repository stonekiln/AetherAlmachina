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

        /// <summary>
        /// Modifierを含めた現在のステータスを計算する
        /// </summary>
        /// <returns>ステータスを表す辞書型</returns>
        Dictionary<StatusType, float> CalcStatus()
        {
            //TODO:パラメータ毎に数値を計算するように変更すること
            Dictionary<StatusType, float> result = new();
            Dictionary<StatusType, float> flat = Modifiers[typeof(FlatModifierParameter)].Value;
            Dictionary<StatusType, float> rate = Modifiers[typeof(RateModifierParameter)].Value;

            foreach (StatusType type in BaseStatus.Keys)
            {
                result[type] = (BaseStatus[type] + flat[type]) * rate[type];
            }

            return result;
        }
        /// <summary>
        /// 指定した種類のステータスの数値を取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>パラメータの数値</returns>
        public float Get(StatusType type)
        {
            return ModifiedStatus[type];
        }
        /// <summary>
        /// 指定した種類のステータスの数値を整数値で取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>パラメータの数値</returns>
        public int GetInt(StatusType type)
        {
            return (int)ModifiedStatus[type];
        }
    }
}