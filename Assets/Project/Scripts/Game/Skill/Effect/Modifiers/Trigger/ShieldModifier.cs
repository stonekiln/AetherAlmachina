using System;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// シールド付与のModifierの定義
    /// </summary>
    public abstract class ShieldModifier : TriggerModifier
    {
        protected override ModifierData TransformData(EnchantExecutionContext context, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    int addShield = Mathf.FloorToInt(data.ModifyValue);
                    if (context.Target.Status.Resource.Shield < addShield)
                    {
                        context.Target.Interaction.ResourceUpdate.Shield.Request
                            .OnNext(new(addShield - context.Target.Status.Resource.Shield));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                curMax =>
                {
                    int curShield = Mathf.FloorToInt(curMax);
                    if (curShield < context.Target.Status.Resource.Shield)
                    {
                        context.Target.Interaction.ResourceUpdate.Shield.Request.OnNext(new(curShield - context.Target.Status.Resource.Shield));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が解除された。");
                })
            {
                ModifierType = typeof(ShieldModifier)
            };
        }
        public override Observable<Unit> Create(EnchantExecutionContext context)
        {
            return context.Target.Interaction.ResourceUpdate.Shield.Response.Where(log => log.Current <= 0).Take(1).AsUnitObservable();
        }
    }
    /// <summary>
    /// 定数シールド付与のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldFlat : ShieldModifier
    {
        public override string DisplayUnit => "";
    }
    /// <summary>
    /// HP割合シールド付与のModifierの定義
    /// </summary>
    [Serializable]
    public class ShieldHPRate : ShieldModifier
    {
        public override string DisplayUnit => "%";
        protected override ModifierData TransformData(EnchantExecutionContext context, ModifierRawData data)
        {
            ModifierRawData modifiedValueData = new(data)
            {
                ModifyValue = context.User.Status.Get(StatusType.MaxHitPoint) * data.ModifyValue / 100f
            };
            return base.TransformData(context, modifiedValueData);
        }
    }
}