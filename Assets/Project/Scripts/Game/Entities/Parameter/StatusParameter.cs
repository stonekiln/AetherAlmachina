using System;
using System.Collections.Generic;
using AetherAlmachina.Skill.Effect.Modifiers;
using DConfig.EntityLife.Event;
using DIVFactor.Event;
using DIVFactor.Extensions;
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

        public ResourceStateParameter(ProcessEventBundle Process, MonoBehaviour monoBehaviour)
        {
            HitPoint = 0;
            Shield = 0;
            Disable = 0;
            Cost = 0;

            Process.ResourceUpdate.HP.Switch(Process.EntityDeath).Subscribe(log =>
            {
                HPUpdate(log.Delta);
                return new();
            }).AddTo(monoBehaviour);
            Process.ResourceUpdate.Shield.Subscribe(log => ShieldUpdate(log.Delta)).AddTo(monoBehaviour);
            Process.ResourceUpdate.Disable.Subscribe(log => DisableUpdate(log.Delta)).AddTo(monoBehaviour);
            Process.CostUpdate.Subscribe(log => CostUpdate(log.Delta)).AddTo(monoBehaviour);
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
        Dictionary<Type, ModifierStock> ModifierStorage { get; init; }

        public StatusParameter(StatusBase status, ResourceStateParameter resourceState, EventBus<HPUpdateEvent> hpUpdate)
        {
            BaseStatus = new(status.BaseStatus);
            ModifierStorage = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter(BaseStatus)},
                {typeof(RateModifierParameter),new RateModifierParameter(BaseStatus)},
                {typeof(TriggerModifiers),new TriggerModifiers()}
            };
            Resource = resourceState;

            hpUpdate.OnNext(new(GetInt(StatusType.MaxHitPoint)));
        }

        public T GetModifiers<T>() where T : ModifierStock
        {
            return (T)ModifierStorage[typeof(T)];
        }
        /// <summary>
        /// 指定した種類のステータスの数値を取得する
        /// </summary>
        /// <param name="type">指定するステータス</param>
        /// <returns>取得した数値</returns>
        public float Get(StatusType type)
        {
            return (BaseStatus[type] + GetModifiers<FlatModifierParameter>().GetValue(type)) * GetModifiers<RateModifierParameter>().GetValue(type);
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