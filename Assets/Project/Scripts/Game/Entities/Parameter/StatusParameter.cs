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

        public ResourceStatusParameter(ProcessEventBundle Process, MonoBehaviour monoBehaviour)
        {
            Cost = 0;
            HitPoint = 0;
            Shield = 0;
            Disable = 0;

            Process.ResourceUpdate.Cost.Request.Switch(Process.ResourceUpdate.Cost.Response).Subscribe(log => new(CostUpdate(log.Delta))).AddTo(monoBehaviour);
            Process.ResourceUpdate.HP.Request.Switch(Process.ResourceUpdate.HP.Response).Subscribe(log => new(HPUpdate(log.Delta))).AddTo(monoBehaviour);
            Process.ResourceUpdate.Shield.Request.Switch(Process.ResourceUpdate.Shield.Response).Subscribe(log => new(ShieldUpdate(log.Delta))).AddTo(monoBehaviour);
            Process.ResourceUpdate.Disable.Request.Switch(Process.ResourceUpdate.Disable.Response).Subscribe(log => new(DisableUpdate(log.Delta))).AddTo(monoBehaviour);
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
    public class StatusParameter : IEnchantableStatus
    {
        public ResourceStatusParameter Resource { get; init; }
        Dictionary<StatusType, float> BaseStatus { get; init; }
        Dictionary<Type, ModifierStock> ModifierStorage { get; init; }

        public StatusParameter(StatusBase status, ResourceStatusParameter resourceState, EventBus<HPUpdateRequestEvent> hpUpdate)
        {
            BaseStatus = new(status.BaseStatus);
            ModifierStorage = new()
            {
                {typeof(FlatModifierParameter),new FlatModifierParameter(BaseStatus)},
                {typeof(RateModifierParameter),new RateModifierParameter(BaseStatus)},
                {typeof(TriggerModifierStock),new TriggerModifierStock()}
            };
            Resource = resourceState;

            hpUpdate.OnNext(new(GetInt(StatusType.MaxHitPoint)));
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