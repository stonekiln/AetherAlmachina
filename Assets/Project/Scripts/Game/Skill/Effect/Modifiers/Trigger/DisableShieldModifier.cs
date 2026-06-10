using System;
using R3;
using UnityEngine;

namespace AetherAlmachina.Skill.Effect.Modifiers
{
    /// <summary>
    /// 攻撃無効化のModifierの定義
    /// </summary>
    [Serializable]
    public class DisableTimesModifier : TriggerModifier
    {
        public override string DisplayUnit => "(回)";
        protected override ModifierData TransformData(EnchantExecutionContext context, ModifierRawData data)
        {
            return new(data,
                preMax =>
                {
                    int addDisable = Mathf.FloorToInt(data.ModifyValue);
                    if (context.Target.Status.Resource.Disable < addDisable)
                    {
                        context.Target.Interaction.ResourceUpdate.Disable.Request
                            .OnNext(new(addDisable - context.Target.Status.Resource.Disable));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が付与された。");
                },
                curMax =>
                {
                    int curDisable = Mathf.FloorToInt(curMax);
                    if (curDisable < context.Target.Status.Resource.Disable)
                    {
                        context.Target.Interaction.ResourceUpdate.Disable.Request
                            .OnNext(new(curDisable - context.Target.Status.Resource.Disable));
                    }

                    Debug.Log(data.TypeData.Name + ":" + data.Value + data.TypeData.DisplayUnit + " の効果が解除された。");
                })
            {
                ModifierType = typeof(DisableTimesModifier)
            };
        }
        public override Observable<Unit> Create(EnchantExecutionContext context)
        {
            return context.Target.Interaction.ResourceUpdate.Disable.Response.Where(log => log.Current <= 0).Take(1).AsUnitObservable();
        }
    }
}