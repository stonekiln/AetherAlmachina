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
        public int MagicPoint { get; private set; }
        Dictionary<StatusType, float> BaseStatus { get; init; }
        public Dictionary<Type, ModifierParameter> Modifiers { get; init; }

        public StatusParameter(StatusBase status)
        {
            BaseStatus = new(status.BaseStatus);
            Modifiers = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter()},
                {typeof(RateModifierParameter),new RateModifierParameter()}
            };
            hitPoint = Get(StatusType.MaxHitPoint);
            MagicPoint = 0;
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
        /// <summary>
        /// MPの値を変更する
        /// </summary>
        /// <param name="delta">MPの変化量</param>
        public void MPUpdate(int delta)
        {
            MagicPoint += delta;
        }
    }
}