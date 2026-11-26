using AetherAlmachina.Skill.Effect.Modifiers;
using DConfig.EntityLife.Event;
using DIVFactor.Lifecycle;
using R3;
using System;
using System.Collections.Generic;

namespace AetherAlmachina.Entities.Parameter
{
    /// <summary>
    /// ステータスがそのパラメータの最大値でその数値に実数値があるパラメータの実数値部分を制御するためのクラス
    /// </summary>
    public class ResourceStatusParameter
    {
        /// <summary>
        /// コストの現在の残量
        /// </summary>
        public int Cost { get; private set; }
        /// <summary>
        /// HPの現在の残量
        /// </summary>
        public int HitPoint { get; private set; }
        /// <summary>
        /// シールドの現在の残量
        /// </summary>
        public int Shield { get; private set; }
        /// <summary>
        /// ダメージ回数無効化の残数
        /// </summary>
        public int Disable { get; private set; }

        public void Subscribe(ResourceUpdateEventHub resourceUpdate, UsageLifetime lifetime)
        {
            resourceUpdate.Cost.AsObservable().Subscribe(log => new(CostUpdate(log.Delta))).AddTo(lifetime);
            resourceUpdate.HP.AsObservable().Subscribe(log => new(HPUpdate(log.Delta))).AddTo(lifetime);
            resourceUpdate.Shield.AsObservable().Subscribe(log => new(ShieldUpdate(log.Delta))).AddTo(lifetime);
            resourceUpdate.Disable.AsObservable().Subscribe(log => new(DisableUpdate(log.Delta))).AddTo(lifetime);
        }

        int HPUpdate(int delta)
        {
            return HitPoint += delta;
        }
        int ShieldUpdate(int delta)
        {
            return Shield += delta;
        }
        int DisableUpdate(int delta)
        {
            return Disable += delta;
        }
        int CostUpdate(int delta)
        {
            return Cost += delta;
        }
    }
    /// <summary>
    /// ステータスのパラメータをコピーして変更可能にするためのクラス
    /// </summary>
    public class StatusParameter
    {
        public ResourceStatusParameter Resource { get; init; }
        Dictionary<StatusType, float> BaseStatus { get; init; }
        Dictionary<Type, ModifierStock> ModifierStorage { get; init; }

        public StatusParameter(StatusBase status, ResourceStatusParameter resourceState)
        {
            BaseStatus = new(status.BaseStatus);
            ModifierStorage = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter(BaseStatus)},
                {typeof(RateModifierParameter),new RateModifierParameter(BaseStatus)},
                {typeof(TriggerModifierStock),new TriggerModifierStock()}
            };
            Resource = resourceState;

        }

        public T GetModifiers<T>() where T : ModifierStock
        {
            return (T)ModifierStorage[typeof(T)];
        }
        /// <summary>
        /// パラメータの実数値を取得する
        /// </summary>
        /// <param name="type">ステータスのパラメータタイプ</param>
        /// <returns>実数値</returns>
        public float Get(StatusType type)
        {
            return (BaseStatus[type] + GetModifiers<FlatModifierParameter>().GetValue(type)) * GetModifiers<RateModifierParameter>().GetValue(type);
        }
        /// <summary>
        /// パラメータの実数値を整数で取得する
        /// </summary>
        /// <param name="type">ステータスのパラメータタイプ</param>
        /// <returns>実数値</returns>
        public int GetInt(StatusType type)
        {
            return (int)Get(type);
        }
    }
}