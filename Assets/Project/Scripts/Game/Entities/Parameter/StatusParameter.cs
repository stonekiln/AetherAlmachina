using System;
using System.Collections.Generic;
using AetherAlmachina.Skill.Effect.Modifiers;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using R3;
using UnityEngine;

namespace AetherAlmachina.Entities.Parameter
{
    public class ResourceStateParameter
    {
        public int HitPoint { get; private set; }
        public int Shield { get; private set; }
        public int Disable { get; private set; }
        public int Cost { get; private set; }

        public ResourceStateParameter(ResourceUpdateEventBundle resourceUpdate, EventBus<CostUpdateEvent> costUpdate, MonoBehaviour monoBehaviour)
        {
            HitPoint = 0;
            Shield = 0;
            Disable = 0;
            Cost = 0;

            resourceUpdate.HP.Subscribe(log => HPUpdate(log.Delta)).AddTo(monoBehaviour);
            resourceUpdate.Shield.Subscribe(log => ShieldUpdate(log.Delta)).AddTo(monoBehaviour);
            resourceUpdate.Disable.Subscribe(log => DisableUpdate(log.Delta)).AddTo(monoBehaviour);
            costUpdate.Subscribe(log => CostUpdate(log.Delta)).AddTo(monoBehaviour);
        }

        /// <summary>
        /// HPの値を変更する
        /// </summary>
        /// <param name="delta">HPの変化量</param>
        void HPUpdate(int delta)
        {
            HitPoint += delta;
        }
        void ShieldUpdate(int delta)
        {
            Shield += delta;
        }
        void DisableUpdate(int delta)
        {
            Disable += delta;
        }
        /// <summary>
        /// MPの値を変更する
        /// </summary>
        /// <param name="delta">MPの変化量</param>
        void CostUpdate(int delta)
        {
            Cost += delta;
        }
    }
    /// <summary>
    /// ステータスのパラメータをコピーして変更可能にするためのクラス
    /// </summary>
    public class StatusParameter
    {
        public ResourceStateParameter Resource { get; init; }
        Dictionary<StatusType, float> BaseStatus { get; init; }
        public Dictionary<Type, ModifierParameter> ModifiedParam { get; init; }
        public TriggerModifiers Triggers { get; init; }

        public StatusParameter(StatusBase status, ResourceStateParameter resourceState, EventBus<HPUpdateEvent> hpUpdate)
        {
            BaseStatus = new(status.BaseStatus);
            ModifiedParam = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter(BaseStatus)},
                {typeof(RateModifierParameter),new RateModifierParameter(BaseStatus)},
            };
            Triggers = new();
            Resource = resourceState;

            hpUpdate.OnNext(new(GetInt(StatusType.MaxHitPoint)));
        }

        /// <summary>
        /// 指定した種類のステータスの数値を取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>取得した数値</returns>
        public float Get(StatusType type)
        {
            return (BaseStatus[type] + ModifiedParam[typeof(FlatModifierParameter)].GetValue(type)) * ModifiedParam[typeof(RateModifierParameter)].GetValue(type);
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